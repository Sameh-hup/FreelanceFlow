using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.DAL.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.PaymentID);
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.Currency).HasConversion<string>().HasMaxLength(10);

        // Restrict delete on invoice with payments (BR-21)
        builder.HasOne(p => p.Invoice)
               .WithMany(i => i.Payments)
               .HasForeignKey(p => p.InvoiceID)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
