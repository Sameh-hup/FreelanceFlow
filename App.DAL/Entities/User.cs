using App.DAL.Enums;

namespace App.DAL.Entities;

public class User
{
    public int UserID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int? ClientId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Client? Client { get; set; } // If Role is Client, links to the Client profile
    public virtual ICollection<Client> OwnedClients { get; set; } = new List<Client>(); // Freelancer's clients
    public virtual ICollection<Invitation> CreatedInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
