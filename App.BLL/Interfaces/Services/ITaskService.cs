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
    bool IsTaskOverdue(TaskItem task);

    // Overloads for int freelancerId / legacy compatibility
    Task<bool> AddMilestoneAsync(Milestone milestone, int freelancerId) => AddMilestoneAsync(milestone, freelancerId.ToString());
    Task<bool> AddTaskAsync(TaskItem task, int freelancerId) => AddTaskAsync(task, freelancerId.ToString());
    Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, int freelancerId) => UpdateTaskStatusAsync(taskId, newStatus, freelancerId.ToString());
    Task<List<TaskItem>> GetOverdueTasksAsync(int freelancerId) => GetOverdueTasksAsync(freelancerId.ToString());
}
