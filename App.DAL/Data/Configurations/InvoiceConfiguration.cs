using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.InvoiceID);
        builder.Property(i => i.Amount).HasPrecision(18, 2);
        builder.Property(i => i.Currency).HasConversion<string>().HasMaxLength(10);

        // Concurrency Token (RowVersion) (Section 7, BR-19)
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.HasOne(i => i.Project)
               .WithMany(p => p.Invoices)
               .HasForeignKey(i => i.ProjectID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
