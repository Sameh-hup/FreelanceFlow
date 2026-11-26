using System.ComponentModel.DataAnnotations;
using App.DAL.Entities;

namespace App.Mvc.ViewModels;

public class ClientViewModel
{
    public int ID { get; set; }

    [Required(ErrorMessage = "Company Name is required")]
    [Display(Name = "Company Name")]
    [MaxLength(150)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public int ProjectCount { get; set; }
    public bool HasPortalAccess { get; set; }
    public string? PortalUserEmail { get; set; }

    public Client ToEntity(string freelancerId) => new()
    {
        ID = ID,
        UserID = freelancerId,
        CompanyName = CompanyName,
        Phone = Phone,
        Address = Address,
        Notes = Notes
    };

    public static ClientViewModel FromEntity(Client client) => new()
    {
        ID = client.ID,
        CompanyName = client.CompanyName,
        Phone = client.Phone,
        Address = client.Address,
        Notes = client.Notes,
        ProjectCount = client.Projects?.Count ?? 0,
        HasPortalAccess = client.PortalUser != null,
        PortalUserEmail = client.PortalUser?.Email
    };
}
