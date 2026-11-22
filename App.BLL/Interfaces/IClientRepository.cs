using App.DAL.Entities;

namespace App.BLL.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<List<Client>> GetClientsByFreelancerIdAsync(string freelancerId);
    Task<Client?> GetClientWithProjectsAsync(int clientId, string freelancerId);
}
