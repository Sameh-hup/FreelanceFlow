using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserID);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        // One-to-One / Unique Client Account linkage
        builder.HasIndex(u => u.ClientId)
               .IsUnique()
               .HasFilter("[ClientId] IS NOT NULL");

        builder.HasOne(u => u.Client)
               .WithOne(c => c.PortalUser)
               .HasForeignKey<User>(u => u.ClientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
