using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.Property(e => e.EnrollmentId).IsRequired();
            builder.HasIndex(e => e.EnrollmentId).IsUnique();

            builder.Property(e => e.Midterm).IsRequired().HasPrecision(5, 2);
            builder.Property(e => e.Practical).IsRequired().HasPrecision(5, 2);
            builder.Property(e => e.Final).IsRequired().HasPrecision(5, 2);
            
 

            builder.HasOne(e=>e.Enrollment)
                .WithOne(e=>e.Grade)
                .HasForeignKey<Grade>(g=>g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
