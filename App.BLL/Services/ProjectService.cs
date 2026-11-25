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
            projects = projects
                .Where(p => p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return projects;
    }

    public async Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId)
    {
        return await _projectRepository.GetProjectDetailsAsync(projectId, freelancerId);
    }

    public async Task<bool> CreateProjectAsync(Project project, string freelancerId)
    {
        var canAccessClient = await _accessService.CanFreelancerAccessClientAsync(freelancerId, project.ClientID);
        if (!canAccessClient)
        {
            return false;
        }

        if (project.Deadline <= project.StartDate)
        {
            throw new ArgumentException("Deadline must be after start date.");
        }

        project.Status = ProjectStatus.NotStarted;

        if (!Enum.IsDefined(typeof(Currency), project.Currency))
        {
            throw new ArgumentException("Currency must be EGP, USD, or EUR.");
        }

        project.CreatedAtUtc = _clock.UtcNow;

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProjectStatusAsync(int projectId, ProjectStatus newStatus, string freelancerId)
    {
        var canAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, projectId);
        if (!canAccess)
        {
            return false;
        }

        var project = await _projectRepository.GetByIdAsync(projectId);
        if (project == null)
        {
            return false;
        }

        project.Status = newStatus;

        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, string freelancerId)
    {
        var canAccess = await _accessService.CanFreelancerAccessProjectAsync(freelancerId, projectId);
        if (!canAccess)
        {
            return false;
        }

        var project = await _projectRepository.GetProjectDetailsAsync(projectId, freelancerId);
        if (project == null)
        {
            return false;
        }

        if (project.Invoices != null && project.Invoices.Any())
        {
            throw new InvalidOperationException("Project has existing invoices and cannot be deleted. Status can only be changed to Cancelled.");
        }

        _projectRepository.Delete(project);
        await _projectRepository.SaveChangesAsync();
        return true;
    }

    public int CalculateProgress(Project project)
    {
        var allTasks = project.Milestones?.SelectMany(m => m.Tasks).ToList() ?? new List<TaskItem>();

        if (allTasks.Count == 0)
        {
            return 0;
        }

        var completedCount = allTasks.Count(t => t.Status == TaskItemStatus.Completed);
        var progress = (completedCount / (double)allTasks.Count) * 100.0;

        return (int)Math.Round(progress);
    }

    public ProjectHealth CalculateHealth(Project project)
    {
        if (project.Status != ProjectStatus.NotStarted &&
            project.Status != ProjectStatus.InProgress &&
            project.Status != ProjectStatus.OnHold)
        {
            return ProjectHealth.Healthy;
        }

        var allTasks = project.Milestones?.SelectMany(m => m.Tasks).ToList() ?? new List<TaskItem>();

        if (allTasks.Count == 0)
        {
            return ProjectHealth.Healthy;
        }

        var today = _clock.CairoToday;

        int overdueCount = allTasks.Count(t => t.Status != TaskItemStatus.Completed && t.DueDate < today);

        int daysUntilDeadline = project.Deadline.DayNumber - today.DayNumber;

        int completedCount = allTasks.Count(t => t.Status == TaskItemStatus.Completed);
        double exactProgress = (completedCount / (double)allTasks.Count) * 100.0;

        if (overdueCount >= 3 ||
            (daysUntilDeadline <= 3 && exactProgress < 50.0) ||
            (daysUntilDeadline < 0 && exactProgress < 100.0))
        {
            return ProjectHealth.Critical;
        }

        if ((overdueCount == 1 || overdueCount == 2) ||
            (daysUntilDeadline <= 7 && exactProgress < 70.0))
        {
            return ProjectHealth.AttentionRequired;
        }

        return ProjectHealth.Healthy;
    }
}
