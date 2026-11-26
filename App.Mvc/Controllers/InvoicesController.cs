using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Freelancer")]
public class InvoicesController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IProjectService _projectService;
    private readonly UserManager<User> _userManager;

    public InvoicesController(
        IInvoiceService invoiceService,
        IProjectService projectService,
        UserManager<User> userManager)
    {
        _invoiceService = invoiceService;
        _projectService = projectService;
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

        var invoices = await _invoiceService.GetInvoicesAsync(freelancerId, search);
        var models = invoices.Select(i =>
        {
            var balance = _invoiceService.CalculateRemainingBalance(i);
            var status = _invoiceService.CalculateInvoiceStatus(i);
            return InvoiceViewModel.FromEntity(i, balance, status);
        }).ToList();

        ViewBag.Search = search;
        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? projectId)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var projects = await _projectService.GetProjectsAsync(freelancerId);
        ViewBag.Projects = projects;

        var model = new InvoiceViewModel
        {
            ProjectID = projectId ?? 0,
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14))
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceViewModel model)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        if (!ModelState.IsValid)
        {
            ViewBag.Projects = await _projectService.GetProjectsAsync(freelancerId);
            return View(model);
        }

        try
        {
            var entity = model.ToEntity();
            var success = await _invoiceService.CreateInvoiceAsync(entity, freelancerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Invoice created successfully.";
                return RedirectToAction(nameof(Details), new { id = entity.ID });
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewBag.Projects = await _projectService.GetProjectsAsync(freelancerId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var invoice = await _invoiceService.GetInvoiceDetailsAsync(id);
        if (invoice == null || invoice.Project?.Client?.UserID != freelancerId || invoice.Project.Client.IsDeleted)
            return NotFound();

        var balance = _invoiceService.CalculateRemainingBalance(invoice);
        var status = _invoiceService.CalculateInvoiceStatus(invoice);
        var model = InvoiceViewModel.FromEntity(invoice, balance, status);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(PaymentViewModel model)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        try
        {
            var success = await _invoiceService.RecordPaymentAsync(
                model.InvoiceID,
                model.Amount,
                model.PaymentDate,
                model.Notes,
                freelancerId);

            if (success)
            {
                TempData["SuccessMessage"] = $"Payment of {model.Amount:C} successfully recorded!";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not record payment.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = model.InvoiceID });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        try
        {
            var success = await _invoiceService.DeleteInvoiceAsync(id, freelancerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Invoice deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete invoice.";
            }
        }
        catch (InvalidOperationException ex)
        {
            // BR-21: Invoice has payments
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
