using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.TaskID);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        // Cascade delete: Milestone -> Tasks (Section 7)
        builder.HasOne(t => t.Milestone)
               .WithMany(m => m.Tasks)
               .HasForeignKey(t => t.MilestoneID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
