using App.DAL.Enums;

namespace App.DAL.Entities;

public class Payment
{
    public int PaymentID { get; set; }
    public int InvoiceID { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}
