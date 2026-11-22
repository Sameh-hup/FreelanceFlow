using App.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserID);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

            // One-to-One / Unique Client Account linkage
            entity.HasIndex(u => u.ClientId)
                  .IsUnique()
                  .HasFilter("[ClientId] IS NOT NULL");

            entity.HasOne(u => u.Client)
                  .WithOne(c => c.PortalUser)
                  .HasForeignKey<User>(u => u.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 2. Clients
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.ClientID);
            entity.Property(c => c.CompanyName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Phone).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Address).IsRequired().HasMaxLength(255);

            // Soft delete query filter (BR-05)
            entity.HasQueryFilter(c => !c.IsDeleted);

            // Freelancer owns clients
            entity.HasOne(c => c.Freelancer)
                  .WithMany(u => u.OwnedClients)
                  .HasForeignKey(c => c.UserID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 3. Projects
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.ProjectID);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Budget).HasPrecision(18, 2);
            entity.Property(p => p.Currency).HasConversion<string>().HasMaxLength(10);
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(p => p.Client)
                  .WithMany(c => c.Projects)
                  .HasForeignKey(p => p.ClientID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 4. Milestones
        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.HasKey(m => m.MilestoneID);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(150);

            // Cascade delete: Project -> Milestones (Section 7)
            entity.HasOne(m => m.Project)
                  .WithMany(p => p.Milestones)
                  .HasForeignKey(m => m.ProjectID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 5. Tasks (Cascade delete: Milestone -> Tasks)
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.TaskID);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
            entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(t => t.Milestone)
                  .WithMany(m => m.Tasks)
                  .HasForeignKey(t => t.MilestoneID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 6. Invoices
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.InvoiceID);
            entity.Property(i => i.Amount).HasPrecision(18, 2);
            entity.Property(i => i.Currency).HasConversion<string>().HasMaxLength(10);

            // Concurrency Token (RowVersion) (Section 7, BR-19)
            entity.Property(i => i.RowVersion).IsRowVersion();

            entity.HasOne(i => i.Project)
                  .WithMany(p => p.Invoices)
                  .HasForeignKey(i => i.ProjectID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. Payments
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.PaymentID);
            entity.Property(p => p.Amount).HasPrecision(18, 2);
            entity.Property(p => p.Currency).HasConversion<string>().HasMaxLength(10);

            // Invoice has many payments, restrict delete (BR-21)
            entity.HasOne(p => p.Invoice)
                  .WithMany(i => i.Payments)
                  .HasForeignKey(p => p.InvoiceID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 8. Invitations
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(inv => inv.InvitationID);
            entity.Property(inv => inv.Email).IsRequired().HasMaxLength(255);
            entity.Property(inv => inv.TokenHash).IsRequired().HasMaxLength(255);
            entity.HasIndex(inv => inv.TokenHash).IsUnique();

            entity.HasOne(inv => inv.Client)
                  .WithMany(c => c.Invitations)
                  .HasForeignKey(inv => inv.ClientID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(inv => inv.CreatedByUser)
                  .WithMany(u => u.CreatedInvitations)
                  .HasForeignKey(inv => inv.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 9. AuditLogs
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.AuditLogID);
            entity.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Action).IsRequired().HasMaxLength(50);

            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserID)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
