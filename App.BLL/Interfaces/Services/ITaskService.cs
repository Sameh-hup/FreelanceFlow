using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface ITaskService
{
    Task<bool> AddMilestoneAsync(Milestone milestone, int freelancerId);
    Task<bool> AddTaskAsync(TaskItem task, int freelancerId);
    Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, int freelancerId);
    Task<List<TaskItem>> GetOverdueTasksAsync(int freelancerId);
    MilestoneStatus CalculateMilestoneStatus(Milestone milestone);
}
