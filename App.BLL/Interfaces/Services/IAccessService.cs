namespace App.BLL.Interfaces.Services;

public interface IAccessService
{
    Task<bool> CanFreelancerAccessClientAsync(string freelancerId, int clientId);
    Task<bool> CanFreelancerAccessProjectAsync(string freelancerId, int projectId);
    Task<bool> CanClientAccessProjectAsync(string clientUserId, int projectId);
}
