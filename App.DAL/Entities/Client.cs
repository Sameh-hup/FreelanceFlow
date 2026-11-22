namespace App.DAL.Entities;

public class Client
{
    public int ClientID { get; set; }
    public string UserID { get; set; } = string.Empty; // The Freelancer (User.Id) who owns this client
    public string CompanyName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }

    // Navigation properties
    public virtual User Freelancer { get; set; } = null!;
    public virtual User? PortalUser { get; set; }
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
