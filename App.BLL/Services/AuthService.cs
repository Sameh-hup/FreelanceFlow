using System.Security.Cryptography;
using System.Text;
using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly IClock _clock;
    private readonly IConfiguration? _configuration;

    public AuthService(
        UserManager<User> userManager,
        AppDbContext context,
        IClock clock,
        IConfiguration? configuration = null)
    {
        _userManager = userManager;
        _context = context;
        _clock = clock;
        _configuration = configuration;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user == null || !user.IsActive)
            return null;

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        return isPasswordValid ? user : null;
    }

    public async Task<bool> RegisterFreelancerAsync(string name, string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser != null)
            return false;

        var user = new User
        {
            FullName = name.Trim(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
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
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.ID == clientId && c.UserID == freelancerId && !c.IsDeleted);

        if (client == null)
            throw new UnauthorizedAccessException("Client not found or you do not have permission to invite this client.");

        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        string rawToken = Convert.ToHexString(randomBytes).ToLowerInvariant();
        string tokenHash = ComputeSha256Hash(rawToken);

        var previousInvitations = await _context.Invitation
            .Where(i => i.ClientID == clientId && i.UsedAtUtc == null && i.RevokedAtUtc == null)
            .ToListAsync();

        foreach (var inv in previousInvitations)
        {
            inv.RevokedAtUtc = _clock.UtcNow;
        }

        var invitation = new Invitation
        {
            ClientID = clientId,
            Email = clientEmail.Trim().ToLowerInvariant(),
            TokenHash = tokenHash,
            ExpiresAtUtc = _clock.UtcNow.AddDays(7),
            CreatedByUserId = freelancerId
        };

        await _context.Invitation.AddAsync(invitation);
        await _context.SaveChangesAsync();

        string baseUrl = _configuration?["App:BaseUrl"]?.TrimEnd('/') ?? "";
        return string.IsNullOrEmpty(baseUrl)
            ? $"/Account/ClientInvite?token={rawToken}"
            : $"{baseUrl}/Account/ClientInvite?token={rawToken}";
    }

    public async Task<bool> RegisterClientByInvitationTokenAsync(string token, string name, string password)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        string tokenHash = ComputeSha256Hash(token.Trim());

        var invitation = await _context.Invitation
            .Include(i => i.Client)
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash &&
                                      i.UsedAtUtc == null &&
                                      i.RevokedAtUtc == null &&
                                      i.ExpiresAtUtc > _clock.UtcNow);

        if (invitation == null || (invitation.Client != null && invitation.Client.IsDeleted))
            return false;

        var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
        if (existingUser != null)
            return false;

        var user = new User
        {
            UserName = invitation.Email,
            Email = invitation.Email,
            FullName = name.Trim(),
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

        invitation.UsedAtUtc = _clock.UtcNow;
        if (invitation.Client != null)
        {
            invitation.Client.PortalUser = user;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        => ChangePasswordAsync(userId.ToString(), oldPassword, newPassword);

    public Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, int freelancerId)
        => GenerateClientInvitationLinkAsync(clientId, clientEmail, freelancerId.ToString());

    private static string ComputeSha256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
