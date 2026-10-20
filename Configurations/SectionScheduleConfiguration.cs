
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    public class SectionScheduleConfiguration : IEntityTypeConfiguration<SectionSchedule>
    {
        public void Configure(EntityTypeBuilder<SectionSchedule> builder)
        {
            builder.HasKey(e => e.ScheduleId);
            builder.Property(e => e.SectionId).IsRequired();
            builder.Property(e => e.DayOfWeek).IsRequired();
            builder.Property(e => e.StartTime).IsRequired();
            builder.Property(e => e.EndTime).IsRequired();
            builder.HasIndex(e => new { e.SectionId, e.DayOfWeek, e.StartTime }).IsUnique();
            builder.ToTable(e =>
            {
                e.HasCheckConstraint("CK_SectionSchedule_DayOFWeek", "[DayOfWeek] IN (1,2,3,4,5,6)");
                
                
            });
            builder.HasOne(e => e.Section)
                .WithMany(e => e.SectionSchedules)
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
