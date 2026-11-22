using App.DAL.Enums;

namespace App.DAL.Entities;

public class Project
{
    public int ID { get; set; }
    public int ClientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly Deadline { get; set; }
    public decimal Budget { get; set; }
    public Currency Currency { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Client Client { get; set; } = null!;
    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
