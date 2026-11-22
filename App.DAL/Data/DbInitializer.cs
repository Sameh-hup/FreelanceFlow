using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Ensure migrations are applied
        await context.Database.MigrateAsync();

        // 2. Seed initial Admin (FR-04) if not exists
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var adminUser = new User
            {
                Name = "System Admin",
                Email = "admin@freelanceflow.com",
                // Initial temporary password hash (MustChangePassword = true per FR-04)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                Role = UserRole.Admin,
                IsActive = true,
                MustChangePassword = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
