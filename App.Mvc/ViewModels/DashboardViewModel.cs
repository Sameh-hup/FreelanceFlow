using App.DAL.Entities;

namespace App.Mvc.ViewModels;

public class DashboardViewModel
{
    public int TotalClients { get; set; }
    public int ActiveProjects { get; set; }
    public int OverdueTasks { get; set; }
    public int UnpaidInvoices { get; set; }

    public List<Project> RecentProjects { get; set; } = new();
    public List<Invoice> RecentInvoices { get; set; } = new();
    public List<TaskItem> OverdueTaskList { get; set; } = new();
}
