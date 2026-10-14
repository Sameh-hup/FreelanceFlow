using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic_System.Entities;

namespace University_Academic_System.Configurations;

public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
{
    public void Configure(EntityTypeBuilder<CourseSection> builder)
    {
        builder.Property(e => e.SectionNumber)
            .IsRequired();

        builder.Property(e => e.Capacity)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_CourseSection_Capacity",
            "[Capacity] > 0"));

        // Section belongs to one Instructor
        builder.HasOne(s => s.Instructor)
            .WithMany()
            .HasForeignKey(s => s.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}