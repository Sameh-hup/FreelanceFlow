namespace App.BLL.Interfaces.Services;

public interface IAccessService
{
    Task<bool> CanFreelancerAccessClientAsync(int freelancerId, int clientId);
    Task<bool> CanFreelancerAccessProjectAsync(int freelancerId, int projectId);
    Task<bool> CanClientAccessProjectAsync(int clientUserId, int projectId);
}
