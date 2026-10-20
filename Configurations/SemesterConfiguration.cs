using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Enums;
using University_Academic.Entities;

namespace University_Academic.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.Property(e => e.Name).IsRequired();
        
        

        builder.Property(e => e.AcademicYear).IsRequired()
            .HasMaxLength(9);
        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate).IsRequired();
        builder.Property(e => e.Status).IsRequired()
            .HasDefaultValue(SemesterStatus.Upcoming);
        builder.HasIndex(e => new { e.Name, e.AcademicYear }).IsUnique();

        builder.ToTable(e => 
        {
            e.HasCheckConstraint("CK_Semester_Name", "[Name] IN (1,2,3)");
            e.HasCheckConstraint("CK_Semester_EndDate", "[EndDate] > [StartDate]");
            e.HasCheckConstraint("CK_Semester_Status", "[Status] IN (1,2,3) ");
        });

    }
}
