

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    internal class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(e => e.Code)
                .IsUnique();
            builder.Property(e => e.Description)
                .HasMaxLength(300);

            builder.Property(e => e.CreditHours)
                .IsRequired();

            builder.Property(e => e.Level).IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                "CK_Course_CreditHours",
                "[CreditHours] >= 1 AND [CreditHours] <= 6");
                t.HasCheckConstraint(
                "CK_Course_Level",
                "[Level] IN (1,2,3,4)");

            });
            builder.Property(e=>e.DepartmentId).IsRequired();


            builder.HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
