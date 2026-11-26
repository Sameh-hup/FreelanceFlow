using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Client")]
public class PortalController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IProjectRepository _projectRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProjectService _projectService;
    private readonly IInvoiceService _invoiceService;

    public PortalController(
        UserManager<User> userManager,
        IProjectRepository projectRepository,
        IInvoiceRepository invoiceRepository,
        IProjectService projectService,
        IInvoiceService invoiceService)
    {
        _userManager = userManager;
        _projectRepository = projectRepository;
        _invoiceRepository = invoiceRepository;
        _projectService = projectService;
        _invoiceService = invoiceService;
    }

    private async Task<int?> GetClientIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.ClientId;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var clientId = await GetClientIdAsync();
        if (clientId == null) return Forbid();

        var projects = await _projectRepository.GetProjectsByClientIdAsync(clientId.Value);
        var invoices = await _invoiceRepository.GetInvoicesByClientIdAsync(clientId.Value);

        ViewBag.Projects = projects;
        ViewBag.Invoices = invoices;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Projects()
    {
        var clientId = await GetClientIdAsync();
        if (clientId == null) return Forbid();

        var projects = await _projectRepository.GetProjectsByClientIdAsync(clientId.Value);
        var models = projects.Select(p =>
        {
            var progress = _projectService.CalculateProgress(p);
            var health = _projectService.CalculateHealth(p);
            return ProjectViewModel.FromEntity(p, progress, health);
        }).ToList();

        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> ProjectDetails(int id)
    {
        var clientId = await GetClientIdAsync();
        if (clientId == null) return Forbid();

        var projects = await _projectRepository.GetProjectsByClientIdAsync(clientId.Value);
        var project = projects.FirstOrDefault(p => p.ID == id);
        if (project == null) return NotFound();

        var progress = _projectService.CalculateProgress(project);
        var health = _projectService.CalculateHealth(project);
        var model = ProjectViewModel.FromEntity(project, progress, health);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Invoices()
    {
        var clientId = await GetClientIdAsync();
        if (clientId == null) return Forbid();

        var invoices = await _invoiceRepository.GetInvoicesByClientIdAsync(clientId.Value);
        var models = invoices.Select(i =>
        {
            var balance = _invoiceService.CalculateRemainingBalance(i);
            var status = _invoiceService.CalculateInvoiceStatus(i);
            return InvoiceViewModel.FromEntity(i, balance, status);
        }).ToList();

        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> InvoiceDetails(int id)
    {
        var clientId = await GetClientIdAsync();
        if (clientId == null) return Forbid();

        var invoices = await _invoiceRepository.GetInvoicesByClientIdAsync(clientId.Value);
        var invoice = invoices.FirstOrDefault(i => i.ID == id);
        if (invoice == null) return NotFound();

        var balance = _invoiceService.CalculateRemainingBalance(invoice);
        var status = _invoiceService.CalculateInvoiceStatus(invoice);
        var model = InvoiceViewModel.FromEntity(invoice, balance, status);

        return View(model);
    }
}
