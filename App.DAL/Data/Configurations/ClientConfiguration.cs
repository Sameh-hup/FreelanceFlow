using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.ID);
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Phone).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(255);

        // Soft delete query filter 
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Freelancer owns clients
        builder.HasOne(c => c.Freelancer)
               .WithMany(u => u.OwnedClients)
               .HasForeignKey(c => c.UserID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
