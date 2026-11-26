using App.DAL.Entities;

namespace App.BLL.Interfaces.Services;

public interface IClientService
{
    Task<List<Client>> GetClientsAsync(string freelancerId, string? searchName = null);
    Task<Client?> GetClientByIdAsync(int clientId, string freelancerId);
    Task<bool> CreateClientAsync(Client client, string freelancerId);
    Task<bool> UpdateClientAsync(Client client, string freelancerId);
    Task<bool> SoftDeleteClientAsync(int clientId, string freelancerId);

    // Overloads for int freelancerId / legacy compatibility
    Task<List<Client>> GetClientsAsync(int freelancerId, string? searchName = null) => GetClientsAsync(freelancerId.ToString(), searchName);
    Task<Client?> GetClientByIdAsync(int clientId, int freelancerId) => GetClientByIdAsync(clientId, freelancerId.ToString());
    Task<bool> CreateClientAsync(Client client, int freelancerId) => CreateClientAsync(client, freelancerId.ToString());
    Task<bool> UpdateClientAsync(Client client, int freelancerId) => UpdateClientAsync(client, freelancerId.ToString());
    Task<bool> SoftDeleteClientAsync(int clientId, int freelancerId) => SoftDeleteClientAsync(clientId, freelancerId.ToString());
}
