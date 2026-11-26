using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.DAL.Enums;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Freelancer")]
public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly UserManager<User> _userManager;

    public TasksController(
        ITaskService taskService,
        UserManager<User> userManager)
    {
        _taskService = taskService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddTaskViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        try
        {
            var task = new TaskItem
            {
                MilestoneID = model.MilestoneID,
                Title = model.Title,
                Priority = model.Priority,
                DueDate = model.DueDate,
                SortOrder = model.SortOrder,
                Status = TaskItemStatus.Pending
            };

            var success = await _taskService.AddTaskAsync(task, user.Id);
            if (success)
            {
                TempData["SuccessMessage"] = $"Task '{model.Title}' added successfully.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Details", "Projects", new { id = model.ProjectID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int taskId, TaskItemStatus newStatus, int projectId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var success = await _taskService.UpdateTaskStatusAsync(taskId, newStatus, user.Id);
        if (success)
        {
            TempData["SuccessMessage"] = "Task status updated.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not update task status.";
        }

        return RedirectToAction("Details", "Projects", new { id = projectId });
    }
}
