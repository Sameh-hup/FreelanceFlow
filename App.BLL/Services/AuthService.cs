using App.BLL.Interfaces.Services;
using App.BLL.Services.Clock;
using App.DAL.Data;
using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace App.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly IClock _clock;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<User> userManager,
        AppDbContext context,
        IClock clock,
        IConfiguration configuration)
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
        // Normalizes email to lowercase and trimmed (BR-23)
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Self-registration allowed only for UserRole.Freelancer (FR-01)
        var user = new User
        {
            FullName = name.Trim(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            Role = UserRole.Freelancer,
            IsActive = true,
            CreatedAtUtc = _clock.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<string> GenerateClientInvitationLinkAsync(int clientId, string clientEmail, string freelancerId)
    {
        // Generate a cryptographically secure random token (256 bits = 32 bytes) (FR-05, BR-25)
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        string rawToken = Convert.ToHexString(randomBytes).ToLowerInvariant();

        // Compute TokenHash = SHA256(rawToken)
        string tokenHash = ComputeSha256Hash(rawToken);

        // If a previous unused invitation exists for this client, revoke it (RevokedAtUtc = _clock.UtcNow)
        var previousInvitations = await _context.Invitation
            .Where(i => i.ClientID == clientId && i.UsedAtUtc == null && i.RevokedAtUtc == null)
            .ToListAsync();

        foreach (var inv in previousInvitations)
        {
            inv.RevokedAtUtc = _clock.UtcNow;
        }

        // Save Invitation record with TokenHash and 7-day validity
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

        // Return full invitation URL containing plain token in query string (plain token is never stored in DB)
        string baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "";
        return string.IsNullOrEmpty(baseUrl)
            ? $"/Auth/RegisterClient?token={rawToken}"
            : $"{baseUrl}/Auth/RegisterClient?token={rawToken}";
    }

    public async Task<bool> RegisterClientByInvitationTokenAsync(string token, string name, string password)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // Hash incoming token with SHA256 and look up Invitation
        string tokenHash = ComputeSha256Hash(token.Trim());

        var invitation = await _context.Invitation
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash);

        // Verify invitation exists, UsedAtUtc == null, RevokedAtUtc == null, and ExpiresAtUtc > _clock.UtcNow
        if (invitation == null || invitation.UsedAtUtc != null || invitation.RevokedAtUtc != null || invitation.ExpiresAtUtc <= _clock.UtcNow)
        {
            return false;
        }

        // Create User with Role = UserRole.Client, ClientId = invitation.ClientId
        var user = new User
        {
            FullName = name.Trim(),
            Email = invitation.Email,
            UserName = invitation.Email,
            Role = UserRole.Client,
            ClientId = invitation.ClientID,
            IsActive = true,
            CreatedAtUtc = _clock.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return false;

        // Mark invitation as used: UsedAtUtc = _clock.UtcNow
        invitation.UsedAtUtc = _clock.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static string ComputeSha256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
