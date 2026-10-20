using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    public class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
    {
        public void Configure(EntityTypeBuilder<Classroom> builder)
        {
            builder.Property(e => e.Building)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(e => e.RoomNumber)
                .IsRequired()
                .HasMaxLength(10);
            builder.HasIndex(e => new
            {
                e.Building,
                e.RoomNumber
            }).IsUnique();

            builder.Property(e => e.Capacity)
                .IsRequired();

            builder.ToTable(e => e.HasCheckConstraint("CK_Classroom_Capacity", "[Capacity] >= 1 AND [Capacity] <= 500"));
        }
    }
}
