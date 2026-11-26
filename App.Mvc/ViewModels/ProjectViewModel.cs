using System.ComponentModel.DataAnnotations;
using App.DAL.Entities;
using App.DAL.Enums;

namespace App.Mvc.ViewModels;

public class ProjectViewModel
{
    public int ID { get; set; }

    [Required(ErrorMessage = "Please select a client")]
    [Display(Name = "Client")]
    public int ClientID { get; set; }
    public string? ClientName { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    [Display(Name = "Deadline")]
    public DateOnly Deadline { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));

    [Required]
    [Range(1, 100000000, ErrorMessage = "Budget must be greater than zero")]
    public decimal Budget { get; set; }

    [Required]
    public Currency Currency { get; set; } = Currency.USD;

    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;

    public int Progress { get; set; }
    public ProjectHealth Health { get; set; } = ProjectHealth.Healthy;

    public List<Milestone> Milestones { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();

    public Project ToEntity() => new()
    {
        ID = ID,
        ClientID = ClientID,
        Name = Name,
        Description = Description,
        StartDate = StartDate,
        Deadline = Deadline,
        Budget = Budget,
        Currency = Currency,
        Status = Status
    };

    public static ProjectViewModel FromEntity(Project p, int progress = 0, ProjectHealth health = ProjectHealth.Healthy) => new()
    {
        ID = p.ID,
        ClientID = p.ClientID,
        ClientName = p.Client?.CompanyName,
        Name = p.Name,
        Description = p.Description,
        StartDate = p.StartDate,
        Deadline = p.Deadline,
        Budget = p.Budget,
        Currency = p.Currency,
        Status = p.Status,
        Progress = progress,
        Health = health,
        Milestones = p.Milestones?.OrderBy(m => m.SortOrder).ToList() ?? new(),
        Invoices = p.Invoices?.ToList() ?? new()
    };
}
