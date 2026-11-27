using System.ComponentModel.DataAnnotations;

namespace App.Mvc.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; } = true;
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class InviteClientViewModel
{
    public int ClientID { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Client Email is required")]
    [EmailAddress]
    public string ClientEmail { get; set; } = string.Empty;

    public string? GeneratedInvitationLink { get; set; }
}

public class RegisterClientByInviteViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Your Full Name is required")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ProfileViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    // Read-only info shown on the page
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTime MemberSinceUtc { get; set; }
    public int Clients { get; set; }
    public int Projects { get; set; }
    public int Invoices { get; set; }
}
