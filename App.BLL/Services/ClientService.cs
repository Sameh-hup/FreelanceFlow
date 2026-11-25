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
            clients = clients
                .Where(c => c.CompanyName.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return clients;
    }

    public async Task<Client?> GetClientByIdAsync(int clientId, string freelancerId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.IsDeleted || client.UserID != freelancerId)
        {
            return null;
        }

        return client;
    }

    public async Task<bool> CreateClientAsync(Client client, string freelancerId)
    {
        client.UserID = freelancerId;
        client.CreatedAtUtc = _clock.UtcNow;
        client.IsDeleted = false;
        client.DeletedAtUtc = null;

        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateClientAsync(Client client, string freelancerId)
    {
        var canAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, client.ID);
        if (!canAccess)
        {
            return false;
        }

        var existing = await _clientRepository.GetByIdAsync(client.ID);
        if (existing == null || existing.IsDeleted)
        {
            return false;
        }

        existing.CompanyName = client.CompanyName;
        existing.Phone = client.Phone;
        existing.Address = client.Address;
        existing.Notes = client.Notes;

        _clientRepository.Update(existing);
        await _clientRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteClientAsync(int clientId, string freelancerId)
    {
        var canAccess = await _accessService.CanFreelancerAccessClientAsync(freelancerId, clientId);
        if (!canAccess)
        {
            return false;
        }

        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.IsDeleted)
        {
            return false;
        }

        client.IsDeleted = true;
        client.DeletedAtUtc = _clock.UtcNow;

        var linkedUsers = await _context.Users
            .Where(u => u.ClientId == clientId)
            .ToListAsync();

        foreach (var user in linkedUsers)
        {
            user.IsActive = false;
        }

        _clientRepository.Update(client);
        await _context.SaveChangesAsync();
        return true;
    }
}
