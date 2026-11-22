namespace App.DAL.Entities;

public class Milestone
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
