using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.DAL.Enums;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Freelancer")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IClientService _clientService;
    private readonly ITaskService _taskService;
    private readonly UserManager<User> _userManager;

    public ProjectsController(
        IProjectService projectService,
        IClientService clientService,
        ITaskService taskService,
        UserManager<User> userManager)
    {
        _projectService = projectService;
        _clientService = clientService;
        _taskService = taskService;
        _userManager = userManager;
    }

    private async Task<string?> GetFreelancerIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.Id;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var projects = await _projectService.GetProjectsAsync(freelancerId, search);
        var models = projects.Select(p =>
        {
            var progress = _projectService.CalculateProgress(p);
            var health = _projectService.CalculateHealth(p);
            return ProjectViewModel.FromEntity(p, progress, health);
        }).ToList();

        ViewBag.Search = search;
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? clientId)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var clients = await _clientService.GetClientsAsync(freelancerId);
        ViewBag.Clients = clients;

        var model = new ProjectViewModel
        {
            ClientID = clientId ?? 0,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        if (!ModelState.IsValid)
        {
            ViewBag.Clients = await _clientService.GetClientsAsync(freelancerId);
            return View(model);
        }

        try
        {
            var entity = model.ToEntity();
            var success = await _projectService.CreateProjectAsync(entity, freelancerId);
            if (success)
            {
                TempData["SuccessMessage"] = $"Project '{model.Name}' created successfully.";
                return RedirectToAction(nameof(Details), new { id = entity.ID });
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewBag.Clients = await _clientService.GetClientsAsync(freelancerId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var project = await _projectService.GetProjectDetailsAsync(id, freelancerId);
        if (project == null)
            return NotFound();

        var progress = _projectService.CalculateProgress(project);
        var health = _projectService.CalculateHealth(project);
        var model = ProjectViewModel.FromEntity(project, progress, health);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ProjectStatus newStatus)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var success = await _projectService.UpdateProjectStatusAsync(id, newStatus, freelancerId);
        if (success)
        {
            TempData["SuccessMessage"] = $"Project status changed to {newStatus}.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not update project status.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        try
        {
            var success = await _projectService.DeleteProjectAsync(id, freelancerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Project deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete project.";
            }
        }
        catch (InvalidOperationException ex)
        {
            // BR-06 / FR-13: Project has invoices
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
