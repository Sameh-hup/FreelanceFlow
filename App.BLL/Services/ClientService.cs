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

        // BR-05 / BR-26: When a client is soft-deleted, their linked user account (Users.ClientId) is immediately blocked
        var linkedUser = await _context.Users.FirstOrDefaultAsync(u => u.ClientId == clientId);
        if (linkedUser != null)
        {
            linkedUser.IsActive = false;
        }

        return await _context.SaveChangesAsync() > 0;
    }

    // Overloads for int freelancerId
    public Task<List<Client>> GetClientsAsync(int freelancerId, string? searchName = null)
        => GetClientsAsync(freelancerId.ToString(), searchName);

    public Task<Client?> GetClientByIdAsync(int clientId, int freelancerId)
        => GetClientByIdAsync(clientId, freelancerId.ToString());

    public Task<bool> CreateClientAsync(Client client, int freelancerId)
        => CreateClientAsync(client, freelancerId.ToString());

    public Task<bool> UpdateClientAsync(Client client, int freelancerId)
        => UpdateClientAsync(client, freelancerId.ToString());

    public Task<bool> SoftDeleteClientAsync(int clientId, int freelancerId)
        => SoftDeleteClientAsync(clientId, freelancerId.ToString());
}
