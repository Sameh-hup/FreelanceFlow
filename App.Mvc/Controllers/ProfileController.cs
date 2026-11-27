using App.DAL.Data;
using App.DAL.Entities;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Mvc.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public ProfileController(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();
        return View(await BuildAsync(user, null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        if (!ModelState.IsValid)
            return View(await BuildAsync(user, model));

        user.FullName = model.FullName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(await BuildAsync(user, model));
        }

        TempData["SuccessMessage"] = "Profile updated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProfileViewModel> BuildAsync(User user, ProfileViewModel? posted)
    {
        var vm = new ProfileViewModel
        {
            FullName = posted?.FullName ?? user.FullName,
            PhoneNumber = posted?.PhoneNumber ?? user.PhoneNumber,
            Email = user.Email ?? string.Empty,
            RoleName = User.IsInRole("Admin") ? "Admin" : User.IsInRole("Client") ? "Client" : "Freelancer",
            MemberSinceUtc = user.CreatedAtUtc
        };

        if (User.IsInRole("Freelancer"))
        {
            vm.Clients = await _context.Clients.CountAsync(c => c.UserID == user.Id && !c.IsDeleted);
            vm.Projects = await _context.Projects.CountAsync(p => p.Client.UserID == user.Id && !p.Client.IsDeleted);
            vm.Invoices = await _context.Invoices.CountAsync(i => i.Project.Client.UserID == user.Id && !i.Project.Client.IsDeleted);
        }
        return vm;
    }
}
