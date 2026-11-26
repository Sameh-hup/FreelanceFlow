using System.ComponentModel.DataAnnotations;
using App.DAL.Entities;
using App.DAL.Enums;

namespace App.Mvc.ViewModels;

public class InvoiceViewModel
{
    public int ID { get; set; }

    [Required(ErrorMessage = "Please select a project")]
    [Display(Name = "Project")]
    public int ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }

    [Required]
    [Range(0.01, 100000000, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    public Currency Currency { get; set; }

    [Required]
    [Display(Name = "Issue Date")]
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required]
    [Display(Name = "Due Date")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public decimal RemainingBalance { get; set; }
    public decimal TotalPaid { get; set; }

    public List<Payment> Payments { get; set; } = new();

    public Invoice ToEntity() => new()
    {
        ID = ID,
        ProjectID = ProjectID,
        Amount = Amount,
        Currency = Currency,
        IssueDate = IssueDate,
        DueDate = DueDate
    };

    public static InvoiceViewModel FromEntity(Invoice i, decimal balance = 0, InvoiceStatus status = InvoiceStatus.Unpaid) => new()
    {
        ID = i.ID,
        ProjectID = i.ProjectID,
        ProjectName = i.Project?.Name,
        ClientName = i.Project?.Client?.CompanyName,
        Amount = i.Amount,
        Currency = i.Currency,
        IssueDate = i.IssueDate,
        DueDate = i.DueDate,
        Status = status,
        RemainingBalance = balance,
        TotalPaid = i.Payments?.Sum(p => p.Amount) ?? 0,
        Payments = i.Payments?.OrderByDescending(p => p.PaymentDate).ToList() ?? new()
    };
}

public class PaymentViewModel
{
    public int InvoiceID { get; set; }

    [Required]
    [Range(0.01, 100000000, ErrorMessage = "Payment amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Payment Date")]
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [MaxLength(255)]
    public string? Notes { get; set; }
    public Currency Currency { get; set; }
    public decimal MaxAllowedAmount { get; set; }
}
