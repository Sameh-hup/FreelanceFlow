using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;
namespace University_Academic.Configurations
{
    public class CollegeConfiguration : IEntityTypeConfiguration<College>
    {
        public void Configure(EntityTypeBuilder<College> builder)
        {
            builder.Property(e=>e.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();

            builder.Property(e => e.Code).IsRequired()
                .HasMaxLength(10);
            builder.HasIndex(e => e.Code).IsUnique();

            builder.Property(e => e.Description)
                .HasMaxLength(300);

        }
    }
}
