using App.DAL.Enums;

namespace App.DAL.Entities;

public class Invoice
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
