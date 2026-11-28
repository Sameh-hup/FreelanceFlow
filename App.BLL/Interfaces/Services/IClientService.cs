using App.DAL.Entities;

namespace App.BLL.Interfaces.Services;

public interface IClientService
{
    Task<List<Client>> GetClientsAsync(string freelancerId, string? searchName = null);
    Task<Client?> GetClientByIdAsync(int clientId, string freelancerId);
    Task<bool> CreateClientAsync(Client client, string freelancerId);
    Task<bool> UpdateClientAsync(Client client, string freelancerId);
    Task<bool> SoftDeleteClientAsync(int clientId, string freelancerId);
}
