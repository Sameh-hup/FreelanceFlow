namespace App.DAL.Entities;

public class AuditLog
{
    public int AuditLogID { get; set; }
    public string? UserID { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityID { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }
}
