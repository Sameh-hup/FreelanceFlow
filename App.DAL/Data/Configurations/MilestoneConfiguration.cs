using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.HasKey(m => m.ID);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(150);

        // Cascade delete: Project -> Milestones 
        builder.HasOne(m => m.Project)
               .WithMany(p => p.Milestones)
               .HasForeignKey(m => m.ProjectID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
