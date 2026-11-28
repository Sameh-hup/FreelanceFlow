using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAccessService _accessService;
    private readonly IClock _clock;

    public ProjectService(
        IProjectRepository projectRepository,
        IClientRepository clientRepository,
        IAccessService accessService,
        IClock clock)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _accessService = accessService;
        _clock = clock;
    }

    public async Task<List<Project>> GetProjectsAsync(string freelancerId, string? searchName = null)
    {
        var projects = await _projectRepository.GetProjectsByFreelancerIdAsync(freelancerId);
        if (!string.IsNullOrWhiteSpace(searchName))
        {
            var search = searchName.Trim();
            projects = projects
                .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        return projects;
    }

    public async Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, projectId);
        if (!hasAccess) return null;

        return await _projectRepository.GetProjectDetailsAsync(projectId, freelancerId);
    }

    public async Task<bool> CreateProjectAsync(Project project, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, project.ClientID);
        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not own the selected client.");

        // Deadline > StartDate
        if (project.Deadline <= project.StartDate)
            throw new ArgumentException("Project Deadline must be strictly after the Start Date.");

        project.Status = ProjectStatus.NotStarted;
        project.CreatedAtUtc = _clock.UtcNow;

        await _projectRepository.AddAsync(project);
        return await _projectRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateProjectStatusAsync(int projectId, ProjectStatus newStatus, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, projectId);
        if (!hasAccess) return false;

        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null) return false;

        // Status transitions are manual only
        project.Status = newStatus;
        _projectRepository.Update(project);
        return await _projectRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, projectId);
        if (!hasAccess) return false;

        var project = await _projectRepository.GetProjectDetailsAsync(projectId, freelancerId);
        if (project == null) return false;

        //  Check if project has any invoices
        if (project.Invoices != null && project.Invoices.Any())
        {
            throw new InvalidOperationException("Cannot delete project with existing invoices. Set status to Cancelled instead.");
        }

        _projectRepository.Delete(project);
        return await _projectRepository.SaveChangesAsync() > 0;
    }

    public int CalculateProgress(Project project)
    {
        if (project.Milestones == null || !project.Milestones.Any())
            return 0;

        var allTasks = project.Milestones
            .Where(m => m.Tasks != null)
            .SelectMany(m => m.Tasks)
            .ToList();

        if (allTasks.Count == 0)
            return 0;

        var completedCount = allTasks.Count(t => t.Status == TaskItemStatus.Completed);
        return (int)Math.Round((completedCount / (double)allTasks.Count) * 100.0);
    }

    public ProjectHealth CalculateHealth(Project project)
    {
        if (project.Status != ProjectStatus.NotStarted &&
            project.Status != ProjectStatus.InProgress &&
            project.Status != ProjectStatus.OnHold)
        {
            return ProjectHealth.Healthy;
        }

        var allTasks = project.Milestones?
            .Where(m => m.Tasks != null)
            .SelectMany(m => m.Tasks)
            .ToList() ?? new List<TaskItem>();

        int totalTasks = allTasks.Count;
        if (totalTasks == 0)
            return ProjectHealth.Healthy;

        var today = _clock.CairoToday;

        int overdueCount = allTasks.Count(t => t.Status != TaskItemStatus.Completed && t.DueDate < today);
        int daysUntilDeadline = project.Deadline.DayNumber - today.DayNumber;
        int completedTasks = allTasks.Count(t => t.Status == TaskItemStatus.Completed);
        double exactProgress = (completedTasks / (double)totalTasks) * 100.0;

        // Priority 1: Critical
        if (overdueCount >= 3 ||
            (daysUntilDeadline <= 3 && exactProgress < 50.0) ||
            (daysUntilDeadline < 0 && exactProgress < 100.0))
        {
            return ProjectHealth.Critical;
        }

        // Priority 2: AttentionRequired
        if (overdueCount == 1 || overdueCount == 2 ||
            (daysUntilDeadline <= 7 && exactProgress < 70.0))
        {
            return ProjectHealth.AttentionRequired;
        }

        // Priority 3: Healthy
        return ProjectHealth.Healthy;
    }

    
}
