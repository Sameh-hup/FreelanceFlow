using App.DAL.Entities;

namespace App.BLL.Interfaces.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<bool> RegisterFreelancerAsync(string name, string email, string password);
    Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, string freelancerId);
    Task<bool> RegisterClientByInvitationTokenAsync(string token, string name, string password);
    }
