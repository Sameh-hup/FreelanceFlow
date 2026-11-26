using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<Milestone> _milestoneRepository;
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IClock _clock;
    private readonly AppDbContext _context;

    public TaskService(
        IRepository<Milestone> milestoneRepository,
        IRepository<TaskItem> taskRepository,
        IProjectRepository projectRepository,
        IClock clock,
        AppDbContext context)
    {
        _milestoneRepository = milestoneRepository;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _clock = clock;
        _context = context;
    }

    public async Task<bool> AddMilestoneAsync(Milestone milestone, string freelancerId)
    {
        var project = await _projectRepository.GetProjectDetailsAsync(milestone.ProjectID, freelancerId);
        if (project == null)
            throw new UnauthorizedAccessException("Project not found or you do not have permission to access it.");

        // BR-02: Milestone.DueDate between Project.StartDate and Project.Deadline inclusive
        if (milestone.DueDate < project.StartDate || milestone.DueDate > project.Deadline)
        {
            throw new ArgumentOutOfRangeException(
                nameof(milestone.DueDate),
                $"Milestone Due Date ({milestone.DueDate}) must be between Project Start Date ({project.StartDate}) and Deadline ({project.Deadline}) inclusive (BR-02).");
        }

        await _milestoneRepository.AddAsync(milestone);
        return await _milestoneRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddTaskAsync(TaskItem task, string freelancerId)
    {
        var milestone = await _context.Milestones
            .Include(m => m.Project)
                .ThenInclude(p => p.Client)
            .FirstOrDefaultAsync(m => m.ID == task.MilestoneID);

        if (milestone == null || milestone.Project.Client.UserID != freelancerId || milestone.Project.Client.IsDeleted)
            throw new UnauthorizedAccessException("Milestone not found or you do not have permission to access it.");

        // BR-03: Task.DueDate <= Milestone.DueDate
        if (task.DueDate > milestone.DueDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(task.DueDate),
                $"Task Due Date ({task.DueDate}) cannot exceed Milestone Due Date ({milestone.DueDate}) (BR-03).");
        }

        task.Status = TaskItemStatus.Pending;

        await _taskRepository.AddAsync(task);
        return await _taskRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, string freelancerId)
    {
        var task = await _context.Tasks
            .Include(t => t.Milestone)
                .ThenInclude(m => m.Project)
                    .ThenInclude(p => p.Client)
            .FirstOrDefaultAsync(t => t.ID == taskId);

        if (task == null || task.Milestone.Project.Client.UserID != freelancerId || task.Milestone.Project.Client.IsDeleted)
            return false;

        // FR-17: Status update is manual only
        task.Status = newStatus;
        _taskRepository.Update(task);
        return await _taskRepository.SaveChangesAsync() > 0;
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync(string freelancerId)
    {
        var today = _clock.UtcToday;

        // BR-11: Retrieve all incomplete tasks where DueDate < today across all active projects of freelancer
        return await _context.Tasks
            .Include(t => t.Milestone)
                .ThenInclude(m => m.Project)
            .Where(t => t.Milestone.Project.Client.UserID == freelancerId
                        && !t.Milestone.Project.Client.IsDeleted
                        && t.Milestone.Project.Status != ProjectStatus.Cancelled
                        && t.Milestone.Project.Status != ProjectStatus.Completed
                        && t.Status != TaskItemStatus.Completed
                        && t.DueDate < today)
            .ToListAsync();
    }

    public MilestoneStatus CalculateMilestoneStatus(Milestone milestone)
    {
        // BR-12: Pure evaluation in sequential order
        var tasks = milestone.Tasks?.ToList() ?? new List<TaskItem>();

        if (tasks.Count == 0 || tasks.All(t => t.Status == TaskItemStatus.Pending))
        {
            return MilestoneStatus.Pending;
        }

        if (tasks.All(t => t.Status == TaskItemStatus.Completed))
        {
            return MilestoneStatus.Completed;
        }

        return MilestoneStatus.InProgress;
    }

    // Overloads for int freelancerId
    public Task<bool> AddMilestoneAsync(Milestone milestone, int freelancerId)
        => AddMilestoneAsync(milestone, freelancerId.ToString());

    public Task<bool> AddTaskAsync(TaskItem task, int freelancerId)
        => AddTaskAsync(task, freelancerId.ToString());

    public Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, int freelancerId)
        => UpdateTaskStatusAsync(taskId, newStatus, freelancerId.ToString());

    public Task<List<TaskItem>> GetOverdueTasksAsync(int freelancerId)
        => GetOverdueTasksAsync(freelancerId.ToString());
}
