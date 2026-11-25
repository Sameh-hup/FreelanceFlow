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
        var project = await _context.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.ID == milestone.ProjectID && p.Client.UserID == freelancerId && !p.Client.IsDeleted);

        if (project == null)
        {
            return false;
        }

        if (milestone.DueDate < project.StartDate || milestone.DueDate > project.Deadline)
        {
            throw new ArgumentException("Milestone due date must be between project start date and deadline inclusive.");
        }

        await _milestoneRepository.AddAsync(milestone);
        await _milestoneRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTaskAsync(TaskItem task, string freelancerId)
    {
        var milestone = await _context.Milestones
            .Include(m => m.Project)
                .ThenInclude(p => p.Client)
            .FirstOrDefaultAsync(m => m.ID == task.MilestoneID && m.Project.Client.UserID == freelancerId && !m.Project.Client.IsDeleted);

        if (milestone == null)
        {
            return false;
        }

        if (task.DueDate > milestone.DueDate)
        {
            throw new ArgumentException("Task due date cannot exceed milestone due date.");
        }

        task.Status = TaskItemStatus.Pending;

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, string freelancerId)
    {
        var task = await _context.Tasks
            .Include(t => t.Milestone)
                .ThenInclude(m => m.Project)
                    .ThenInclude(p => p.Client)
            .FirstOrDefaultAsync(t => t.ID == taskId && t.Milestone.Project.Client.UserID == freelancerId && !t.Milestone.Project.Client.IsDeleted);

        if (task == null)
        {
            return false;
        }

        task.Status = newStatus;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync(string freelancerId)
    {
        var today = _clock.CairoToday;

        return await _context.Tasks
            .Include(t => t.Milestone)
                .ThenInclude(m => m.Project)
                    .ThenInclude(p => p.Client)
            .Where(t => t.Milestone.Project.Client.UserID == freelancerId &&
                        !t.Milestone.Project.Client.IsDeleted &&
                        (t.Milestone.Project.Status == ProjectStatus.NotStarted ||
                         t.Milestone.Project.Status == ProjectStatus.InProgress ||
                         t.Milestone.Project.Status == ProjectStatus.OnHold) &&
                        t.Status != TaskItemStatus.Completed &&
                        t.DueDate < today)
            .ToListAsync();
    }

    public MilestoneStatus CalculateMilestoneStatus(Milestone milestone)
    {
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

    public bool IsTaskOverdue(TaskItem task)
    {
        return task.Status != TaskItemStatus.Completed && task.DueDate < _clock.CairoToday;
    }
}
