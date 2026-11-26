using System.Security.Cryptography;
using System.Text;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly IClock _clock;

    public AuthService(
        UserManager<User> userManager,
        AppDbContext context,
        IClock clock)
    {
        _userManager = userManager;
        _context = context;
        _clock = clock;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive)
            return null;

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!isValidPassword)
            return null;

        return user;
    }

    public async Task<bool> RegisterFreelancerAsync(string name, string email, string password)
    {
        // FR-01: Public registration only allows Freelancer role
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return false;

        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = name,
            Role = UserRole.Freelancer,
            IsActive = true,
            MustChangePassword = false,
            CreatedAtUtc = _clock.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Freelancer");
            return true;
        }

        return false;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
            return false;

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (result.Succeeded)
        {
            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
            }
            return true;
        }

        return false;
    }

    public async Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, string freelancerId)
    {
        // Verify client exists and belongs to freelancer
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ID == clientId && c.UserID == freelancerId && !c.IsDeleted);

        if (client == null)
            throw new UnauthorizedAccessException("Client not found or you do not have permission to invite this client.");

        // BR-25: Cryptographically secure random token (32 bytes = 64 hex characters)
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var plainToken = Convert.ToHexString(tokenBytes);

        // SHA-256 hash for database storage
        var tokenHashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainToken));
        var tokenHash = Convert.ToHexString(tokenHashBytes);

        var invitation = new Invitation
        {
            ClientID = clientId,
            Email = clientEmail,
            TokenHash = tokenHash,
            ExpiresAtUtc = _clock.UtcNow.AddDays(7), // BR-25: 7 days expiration
            UsedAtUtc = null,
            RevokedAtUtc = null,
            CreatedByUserId = freelancerId
        };

        await _context.Invitation.AddAsync(invitation);
        await _context.SaveChangesAsync();

        return plainToken;
    }

    public async Task<bool> RegisterClientByInvitationTokenAsync(string token, string name, string password)
    {
        // Hash incoming plain token
        var incomingHashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var incomingHash = Convert.ToHexString(incomingHashBytes);

        var invitation = await _context.Invitation
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.TokenHash == incomingHash &&
                                      i.UsedAtUtc == null &&
                                      i.RevokedAtUtc == null &&
                                      i.ExpiresAtUtc > _clock.UtcNow);

        if (invitation == null || invitation.Client.IsDeleted)
            return false;

        var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
        if (existingUser != null)
            return false;

        var user = new User
        {
            UserName = invitation.Email,
            Email = invitation.Email,
            FullName = name,
            Role = UserRole.Client,
            ClientId = invitation.ClientID,
            IsActive = true,
            MustChangePassword = false,
            CreatedAtUtc = _clock.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return false;

        await _userManager.AddToRoleAsync(user, "Client");

        // Mark invitation as used
        invitation.UsedAtUtc = _clock.UtcNow;
        _context.Invitation.Update(invitation);

        // Link portal user on client record
        invitation.Client.PortalUser = user;
        _context.Clients.Update(invitation.Client);

        await _context.SaveChangesAsync();
        return true;
    }

    // Overloads for int IDs
    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        => ChangePasswordAsync(userId.ToString(), oldPassword, newPassword);

    public Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, int freelancerId)
        => GenerateClientInvitationLinkAsync(clientId, clientEmail, freelancerId.ToString());
}
