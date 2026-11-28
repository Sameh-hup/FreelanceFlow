using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.DAL.Enums;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IClientService _clientService;
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IInvoiceService _invoiceService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public HomeController(
        IClientService clientService,
        IProjectService projectService,
        ITaskService taskService,
        IInvoiceService invoiceService,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _clientService = clientService;
        _projectService = projectService;
        _taskService = taskService;
        _invoiceService = invoiceService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(User))
        {
            ViewData["HideSidebar"] = true;
            return View("Landing");
        }

        if (User.IsInRole("Client"))
            return RedirectToAction("Index", "Portal");

        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Admin");

        return await Dashboard();
    }

    [HttpGet]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var freelancerId = user.Id;

        var clients = await _clientService.GetClientsAsync(freelancerId);
        var projects = await _projectService.GetProjectsAsync(freelancerId);
        List<TaskItem> overdueTasks = await _taskService.GetOverdueTasksAsync(freelancerId);
        var invoices = await _invoiceService.GetInvoicesAsync(freelancerId);

        var activeProjects = projects
            .Where(p => p.Status != ProjectStatus.Completed && p.Status != ProjectStatus.Cancelled)
            .ToList();

        var unpaidInvoices = invoices
            .Where(i =>
            {
                var status = _invoiceService.CalculateInvoiceStatus(i);
                return status != InvoiceStatus.Paid;
            })
            .ToList();

        var model = new DashboardViewModel
        {
            TotalClients = clients.Count,
            ActiveProjects = activeProjects.Count,
            OverdueTasks = overdueTasks.Count,
            UnpaidInvoices = unpaidInvoices.Count,
            RecentProjects = projects.OrderByDescending(p => p.CreatedAtUtc).Take(5).ToList(),
            RecentInvoices = invoices.OrderByDescending(i => i.CreatedAtUtc).Take(5).ToList(),
            OverdueTaskList = overdueTasks.Take(5).ToList()
        };

        return View("Index", model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Landing()
    {
        ViewData["HideSidebar"] = true;
        return View();
    }
}
