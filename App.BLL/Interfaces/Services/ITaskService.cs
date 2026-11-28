using App.DAL.Entities;
using App.DAL.Enums;

namespace App.BLL.Interfaces.Services;

public interface ITaskService
{
    Task<bool> AddMilestoneAsync(Milestone milestone, string freelancerId);
    Task<bool> AddTaskAsync(TaskItem task, string freelancerId);
    Task<bool> UpdateTaskAsync(TaskItem updatedTask, string freelancerId);
    Task<bool> DeleteTaskAsync(int updatedTaskId, string freelancerId);
    Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus newStatus, string freelancerId);
    Task<List<TaskItem>> GetOverdueTasksAsync(string freelancerId);
    MilestoneStatus CalculateMilestoneStatus(Milestone milestone);
    bool IsTaskOverdue(TaskItem task);

}
