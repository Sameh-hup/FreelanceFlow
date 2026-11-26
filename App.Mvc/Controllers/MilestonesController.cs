using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Freelancer")]
public class MilestonesController : Controller
{
    private readonly ITaskService _taskService;
    private readonly UserManager<User> _userManager;

    public MilestonesController(
        ITaskService taskService,
        UserManager<User> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddMilestoneViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        try
        {
            var milestone = new Milestone
            {
                ProjectID = model.ProjectID,
                Name = model.Name,
                DueDate = model.DueDate,
                SortOrder = model.SortOrder
            };

            var success = await _taskService.AddMilestoneAsync(milestone, user.Id);
            if (success)
            {
                TempData["SuccessMessage"] = $"Milestone '{model.Name}' added successfully.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Details", "Projects", new { id = model.ProjectID });
    }
}
