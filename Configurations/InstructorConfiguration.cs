using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;
using University_Academic.Enums;

namespace University_Academic.Configurations
{
    internal class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Phone).HasMaxLength(20);
            builder.Property(e => e.AcademicTitle).IsRequired();
            builder.Property(e => e.HireDate).IsRequired();
            builder.Property(e => e.Status).HasDefaultValue(InstructorStatus.Active);
            
            builder.ToTable(e =>
            {
                e.HasCheckConstraint("CK_Instructor_AcademicTitle", "[AcademicTitle] IN (1,2,3,4,5)");
                e.HasCheckConstraint("CK_Instructor_HireDate", "[HireDate] <= GETDATE()");
                e.HasCheckConstraint("CK_Instructor_Status", "[Status] IN (1,2,3)");
            });
        }
    }
}
