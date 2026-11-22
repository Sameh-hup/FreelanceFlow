using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(inv => inv.ID);
        builder.Property(inv => inv.Email).IsRequired().HasMaxLength(255);
        builder.Property(inv => inv.TokenHash).IsRequired().HasMaxLength(255);
        builder.HasIndex(inv => inv.TokenHash).IsUnique();

        builder.HasOne(inv => inv.Client)
               .WithMany(c => c.Invitations)
               .HasForeignKey(inv => inv.ClientID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(inv => inv.CreatedByUser)
               .WithMany(u => u.CreatedInvitations)
               .HasForeignKey(inv => inv.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
