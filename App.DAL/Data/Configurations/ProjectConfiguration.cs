using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.ID);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Budget).HasPrecision(18, 2);
        builder.Property(p => p.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(p => p.Client)
               .WithMany(c => c.Projects)
               .HasForeignKey(p => p.ClientID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
