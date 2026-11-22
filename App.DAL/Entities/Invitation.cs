namespace App.DAL.Entities;

public class Invitation
{
    public int ID { get; set; }
    public int ClientID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    // Navigation properties
    public virtual Client Client { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
}
