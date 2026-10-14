using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic_System.Entities;

namespace University_Academic_System.Configurations;

public class SectionScheduleConfiguration : IEntityTypeConfiguration<SectionSchedule>
{
    public void Configure(EntityTypeBuilder<SectionSchedule> builder)
    {
        builder.Property(e => e.DayOfWeek)
            .IsRequired();

        builder.Property(e => e.StartTime)
            .IsRequired();

        builder.Property(e => e.EndTime)
            .IsRequired();

        
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_SectionSchedule_Duration",
            "[EndTime] > [StartTime] "
            + "AND DATEDIFF(MINUTE, [StartTime], [EndTime]) >= 45"));

        // Schedule belongs to one Section
        builder.HasOne(s => s.Section)
            .WithMany()
            .HasForeignKey(s => s.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}