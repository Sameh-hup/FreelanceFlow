using App.BLL.Interfaces;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IAccessService _accessService;
    private readonly IClock _clock;
    private readonly AppDbContext _context;

    public ClientService(
        IClientRepository clientRepository,
        IAccessService accessService,
        IClock clock,
        AppDbContext context)
    {
        _clientRepository = clientRepository;
        _accessService = accessService;
        _clock = clock;
        _context = context;
    }

    public async Task<List<Client>> GetClientsAsync(string freelancerId, string? searchName = null)
    {
        var clients = await _clientRepository.GetClientsByFreelancerIdAsync(freelancerId);

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            var search = searchName.Trim();
            clients = clients
                .Where(c => c.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return clients;
    }

    public async Task<Client?> GetClientByIdAsync(int clientId, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, clientId);
        if (!hasAccess) return null;

        return await _clientRepository.GetClientWithProjectsAsync(clientId, freelancerId);
    }

    public async Task<bool> CreateClientAsync(Client client, string freelancerId)
    {
        client.UserID = freelancerId;
        client.CreatedAtUtc = _clock.UtcNow;
        client.IsDeleted = false;
        client.DeletedAtUtc = null;

        await _clientRepository.AddAsync(client);
        return await _clientRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateClientAsync(Client client, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, client.ID);
        if (!hasAccess) return false;

        var existingClient = await _clientRepository.GetByIdAsync(client.ID);
        if (existingClient == null || existingClient.IsDeleted || existingClient.UserID != freelancerId)
            return false;

        existingClient.CompanyName = client.CompanyName;
        existingClient.Phone = client.Phone;
        existingClient.Address = client.Address;
        existingClient.Notes = client.Notes;

        _clientRepository.Update(existingClient);
        return await _clientRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> SoftDeleteClientAsync(int clientId, string freelancerId)
    {
        var hasAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, clientId);
        if (!hasAccess) return false;

        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.IsDeleted || client.UserID != freelancerId)
            return false;

        client.IsDeleted = true;
        client.DeletedAtUtc = _clock.UtcNow;
        _clientRepository.Update(client);

        var linkedUsers = await _context.Users
            .Where(u => u.ClientId == clientId)
            .ToListAsync();

        foreach (var user in linkedUsers)
        {
            user.IsActive = false;
        }

        return await _context.SaveChangesAsync() > 0;
    }
}
