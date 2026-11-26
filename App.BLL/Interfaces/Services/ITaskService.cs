using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface ITaskService
{
    Task<bool> AddMilestoneAsync(Milestone milestone, string freelancerId);
    Task<bool> AddTaskAsync(TaskItem task, string freelancerId);
    Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, string freelancerId);
    Task<List<TaskItem>> GetOverdueTasksAsync(string freelancerId);
    MilestoneStatus CalculateMilestoneStatus(Milestone milestone);

    // Overloads for int freelancerId
    Task<bool> AddMilestoneAsync(Milestone milestone, int freelancerId);
    Task<bool> AddTaskAsync(TaskItem task, int freelancerId);
    Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, int freelancerId);
    Task<List<TaskItem>> GetOverdueTasksAsync(int freelancerId);
}
