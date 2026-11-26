using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public AdminController(
        UserManager<User> userManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users.ToListAsync();
        var model = new AdminDashboardViewModel
        {
            TotalUsers = users.Count,
            TotalFreelancers = users.Count(u => u.Role == UserRole.Freelancer),
            TotalClients = users.Count(u => u.Role == UserRole.Client),
            ActiveUsers = users.Count(u => u.IsActive),
            RecentUsers = users.OrderByDescending(u => u.CreatedAtUtc).Take(5).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.OrderByDescending(u => u.CreatedAtUtc).ToListAsync();
        var models = users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email ?? "",
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAtUtc = u.CreatedAtUtc
        }).ToList();

        return View(models);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Admin cannot deactivate himself
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser?.Id == user.Id)
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own admin account.";
            return RedirectToAction(nameof(Users));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        TempData["SuccessMessage"] = $"User '{user.FullName}' has been {(user.IsActive ? "activated" : "deactivated")}.";
        return RedirectToAction(nameof(Users));
    }
}
