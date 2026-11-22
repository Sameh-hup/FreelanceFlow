using App.DAL.Entities;

namespace App.BLL.Interfaces.Services;

public interface IClientService
{
    Task<List<Client>> GetClientsAsync(int freelancerId, string? searchName = null);
    Task<Client?> GetClientByIdAsync(int clientId, int freelancerId);
    Task<bool> CreateClientAsync(Client client, int freelancerId);
    Task<bool> UpdateClientAsync(Client client, int freelancerId);
    Task<bool> SoftDeleteClientAsync(int clientId, int freelancerId);
}
