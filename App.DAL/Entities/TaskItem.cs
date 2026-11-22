using App.DAL.Enums;

namespace App.DAL.Entities;

public class TaskItem
{
    public int ID { get; set; }
    public int MilestoneID { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly DueDate { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual Milestone Milestone { get; set; } = null!;
}
