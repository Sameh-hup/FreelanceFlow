using App.DAL.Entities;

namespace App.BLL.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<List<Project>> GetProjectsByFreelancerIdAsync(int freelancerId);
    Task<Project?> GetProjectDetailsAsync(int projectId, int freelancerId);
    Task<List<Project>> GetProjectsByClientIdAsync(int clientId);
}
