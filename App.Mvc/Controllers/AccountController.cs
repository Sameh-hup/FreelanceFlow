using App.BLL.Interfaces.Services;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace App.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;

    public AccountController(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IAuthService authService,
        AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _authService = authService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToDashboard();
        }
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["HideSidebar"] = true;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["HideSidebar"] = true;
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact support.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            if (user.Role == UserRole.Admin && user.MustChangePassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToDashboard(user.Role);
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToDashboard();
        }
        ViewData["HideSidebar"] = true;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["HideSidebar"] = true;
        if (!ModelState.IsValid)
            return View(model);

        var success = await _authService.RegisterFreelancerAsync(model.FullName, model.Email, model.Password);
        if (success)
        {
            TempData["SuccessMessage"] = "Account registered successfully! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, "An account with this email address already exists.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ClientInvite(string token)
    {
        ViewData["HideSidebar"] = true;
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Invalid invitation token.");
        }

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();
        var invitation = await _context.Invitation
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash && i.UsedAtUtc == null && i.RevokedAtUtc == null);

        if (invitation == null || invitation.ExpiresAtUtc < DateTime.UtcNow || invitation.Client.IsDeleted)
        {
            ViewBag.ErrorMessage = "This invitation is invalid or has expired.";
            return View("InviteError");
        }

        return View(new RegisterClientByInviteViewModel
        {
            Token = token,
            Email = invitation.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClientInvite(RegisterClientByInviteViewModel model)
    {
        ViewData["HideSidebar"] = true;
        if (!ModelState.IsValid)
            return View(model);

        var success = await _authService.RegisterClientByInvitationTokenAsync(model.Token, model.Name, model.Password);
        if (success)
        {
            TempData["SuccessMessage"] = "Your client portal account was created successfully! Please sign in.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, "Unable to complete registration. The invitation may have expired or already been used.");
        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var success = await _authService.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
        if (success)
        {
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToDashboard(user.Role);
        }

        ModelState.AddModelError(string.Empty, "Incorrect current password or invalid new password.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewData["HideSidebar"] = true;
        return View();
    }

    private IActionResult RedirectToDashboard(UserRole? role = null)
    {
        if (role == null)
        {
            if (User.IsInRole("Admin")) role = UserRole.Admin;
            else if (User.IsInRole("Client")) role = UserRole.Client;
            else role = UserRole.Freelancer;
        }

        return role switch
        {
            UserRole.Admin => RedirectToAction("Index", "Admin"),
            UserRole.Client => RedirectToAction("Index", "Portal"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
