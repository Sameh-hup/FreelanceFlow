using App.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace App.DAL.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int? ClientId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Client? Client { get; set; }
    public virtual ICollection<Client> OwnedClients { get; set; } = new List<Client>();
    public virtual ICollection<Invitation> CreatedInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
