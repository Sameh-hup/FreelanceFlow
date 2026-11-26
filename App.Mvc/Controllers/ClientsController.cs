using App.BLL.Interfaces.Services;
using App.DAL.Entities;
using App.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Mvc.Controllers;

[Authorize(Roles = "Freelancer")]
public class ClientsController : Controller
{
    private readonly IClientService _clientService;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public ClientsController(
        IClientService clientService,
        IAuthService authService,
        UserManager<User> userManager)
    {
        _clientService = clientService;
        _authService = authService;
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

        var clients = await _clientService.GetClientsAsync(freelancerId, search);
        var models = clients.Select(ClientViewModel.FromEntity).ToList();

        ViewBag.Search = search;
        return View(models);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ClientViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientViewModel model)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        if (!ModelState.IsValid)
            return View(model);

        var success = await _clientService.CreateClientAsync(model.ToEntity(freelancerId), freelancerId);
        if (success)
        {
            TempData["SuccessMessage"] = $"Client '{model.CompanyName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Unable to save client. Please check your inputs.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var client = await _clientService.GetClientByIdAsync(id, freelancerId);
        if (client == null)
            return NotFound();

        return View(ClientViewModel.FromEntity(client));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClientViewModel model)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        if (!ModelState.IsValid)
            return View(model);

        model.ID = id;
        var success = await _clientService.UpdateClientAsync(model.ToEntity(freelancerId), freelancerId);
        if (success)
        {
            TempData["SuccessMessage"] = $"Client '{model.CompanyName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to update client.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var client = await _clientService.GetClientByIdAsync(id, freelancerId);
        if (client == null)
            return NotFound();

        return View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        var success = await _clientService.SoftDeleteClientAsync(id, freelancerId);
        if (success)
        {
            TempData["SuccessMessage"] = "Client moved to trash.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not delete client.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Invite(int clientId, string clientEmail)
    {
        var freelancerId = await GetFreelancerIdAsync();
        if (freelancerId == null) return Challenge();

        try
        {
            var rawToken = await _authService.GenerateClientInvitationLinkAsync(clientId, clientEmail, freelancerId);
            var activationUrl = Url.Action("ClientInvite", "Account", new { token = rawToken }, Request.Scheme);
            TempData["SuccessMessage"] = $"Invitation generated! Activation link: {activationUrl}";
            TempData["InviteLink"] = activationUrl;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
