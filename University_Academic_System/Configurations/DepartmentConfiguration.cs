using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic_System.Entities;

namespace University_Academic_System.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

        builder.HasIndex(e => new {e.Name ,e.CollegeId})
                .IsUnique();
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(10);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e=>e.Description) 
            .HasMaxLength(300);

        // Department belongs to one College
        builder.HasOne(d => d.College)
            .WithMany(c => c.Departments)
            .HasForeignKey(d => d.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);











        
    }
}
