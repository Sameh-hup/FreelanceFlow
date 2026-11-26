using App.DAL.Entities;

namespace App.BLL.Interfaces.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> RegisterFreelancerAsync(string name, string email, string password);
    Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, string freelancerId);
    Task<bool> RegisterClientByInvitationTokenAsync(string token, string name, string password);

    // Overloads for legacy int compatibility
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword) => ChangePasswordAsync(userId.ToString(), oldPassword, newPassword);
    Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, int freelancerId) => GenerateClientInvitationLinkAsync(clientId, clientEmail, freelancerId.ToString());
}
