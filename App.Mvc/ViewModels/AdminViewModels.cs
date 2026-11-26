using System.ComponentModel.DataAnnotations;
using App.DAL.Entities;
using App.DAL.Enums;

namespace App.Mvc.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalFreelancers { get; set; }
    public int TotalClients { get; set; }
    public int ActiveUsers { get; set; }
    public List<User> RecentUsers { get; set; } = new();
}

public class UserListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class AddMilestoneViewModel
{
    public int ProjectID { get; set; }

    [Required(ErrorMessage = "Milestone name is required")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Due Date")]
    public DateOnly DueDate { get; set; }

    public int SortOrder { get; set; }
}

public class AddTaskViewModel
{
    public int MilestoneID { get; set; }
    public int ProjectID { get; set; }

    [Required(ErrorMessage = "Task title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Required]
    [Display(Name = "Due Date")]
    public DateOnly DueDate { get; set; }

    public int SortOrder { get; set; }
}
