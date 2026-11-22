using App.DAL.Entities;

namespace App.BLL.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<List<Project>> GetProjectsByFreelancerIdAsync(string freelancerId);
    Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId);
    Task<List<Project>> GetProjectsByClientIdAsync(int clientId);
}
