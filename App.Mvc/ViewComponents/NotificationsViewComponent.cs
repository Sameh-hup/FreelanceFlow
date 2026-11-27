using System.Security.Claims;
using App.DAL.Data;
using App.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Mvc.ViewComponents;

public class NotificationItem
{
    public string Icon { get; set; } = "bi-bell";
    public string Tone { get; set; } = "indigo"; // indigo | success | warning | danger
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime When { get; set; }
    public string? Url { get; set; }
}

/// <summary>Builds the "Activity &amp; Alerts" list for the topbar bell (freelancer only).</summary>
public class NotificationsViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public NotificationsViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = new List<NotificationItem>();
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId) && UserClaimsPrincipal.IsInRole("Freelancer"))
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var payments = await _context.Payments
                .Where(p => p.Invoice.Project.Client.UserID == userId && !p.Invoice.Project.Client.IsDeleted)
                .OrderByDescending(p => p.CreatedAtUtc)
                .Take(4)
                .Select(p => new { p.Amount, p.Currency, p.CreatedAtUtc, p.InvoiceID, Client = p.Invoice.Project.Client.CompanyName })
                .ToListAsync();
            foreach (var p in payments)
                items.Add(new NotificationItem
                {
                    Icon = "bi-cash-coin", Tone = "success", Title = "Payment recorded",
                    Text = $"Received {p.Currency} {p.Amount:N2} for invoice #{p.InvoiceID} ({p.Client})",
                    When = p.CreatedAtUtc, Url = $"/Invoices/Details/{p.InvoiceID}"
                });

            var invoices = await _context.Invoices
                .Where(i => i.Project.Client.UserID == userId && !i.Project.Client.IsDeleted)
                .OrderByDescending(i => i.CreatedAtUtc)
                .Take(4)
                .Select(i => new { i.ID, i.Amount, i.Currency, i.CreatedAtUtc, Project = i.Project.Name })
                .ToListAsync();
            foreach (var i in invoices)
                items.Add(new NotificationItem
                {
                    Icon = "bi-receipt", Tone = "indigo", Title = "Invoice created",
                    Text = $"Issued invoice #{i.ID} ({i.Currency} {i.Amount:N2}) for {i.Project}",
                    When = i.CreatedAtUtc, Url = $"/Invoices/Details/{i.ID}"
                });

            var overdueTasks = await _context.Tasks
                .Where(t => t.Milestone.Project.Client.UserID == userId && !t.Milestone.Project.Client.IsDeleted
                            && t.Status != TaskItemStatus.Completed && t.DueDate < today)
                .OrderBy(t => t.DueDate)
                .Take(3)
                .Select(t => new { t.Title, t.DueDate, ProjectId = t.Milestone.ProjectID, Project = t.Milestone.Project.Name })
                .ToListAsync();
            foreach (var t in overdueTasks)
                items.Add(new NotificationItem
                {
                    Icon = "bi-exclamation-triangle-fill", Tone = "danger", Title = "Task overdue",
                    Text = $"\"{t.Title}\" in {t.Project} was due {t.DueDate:MMM dd}",
                    When = t.DueDate.ToDateTime(TimeOnly.MinValue), Url = $"/Projects/Details/{t.ProjectId}"
                });

            var lateProjects = await _context.Projects
                .Where(p => p.Client.UserID == userId && !p.Client.IsDeleted
                            && p.Deadline < today
                            && p.Status != ProjectStatus.Completed && p.Status != ProjectStatus.Cancelled)
                .OrderBy(p => p.Deadline)
                .Take(2)
                .Select(p => new { p.ID, p.Name, p.Deadline })
                .ToListAsync();
            foreach (var p in lateProjects)
                items.Add(new NotificationItem
                {
                    Icon = "bi-flag-fill", Tone = "warning", Title = "Project past deadline",
                    Text = $"{p.Name} was due {p.Deadline:MMM dd, yyyy}",
                    When = p.Deadline.ToDateTime(TimeOnly.MinValue), Url = $"/Projects/Details/{p.ID}"
                });
        }

        return View(items.OrderByDescending(i => i.When).Take(8).ToList());
    }
}
