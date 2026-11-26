using App.DAL.Entities;
using App.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.DAL.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // 1. Seed Roles
        string[] roles = ["Admin", "Freelancer", "Client"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Admin user
        var adminEmail = "admin@flow.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                MustChangePassword = false,
                CreatedAtUtc = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Also check if admin@freelanceflow.com exists
        var legacyAdmin = await userManager.FindByEmailAsync("admin@freelanceflow.com");
        if (legacyAdmin == null)
        {
            var lAdmin = new User
            {
                UserName = "admin@freelanceflow.com",
                Email = "admin@freelanceflow.com",
                FullName = "System Admin",
                Role = UserRole.Admin,
                IsActive = true,
                MustChangePassword = false,
                CreatedAtUtc = DateTime.UtcNow,
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(lAdmin, "Admin@123456");
            if (res.Succeeded) await userManager.AddToRoleAsync(lAdmin, "Admin");
        }

        // 3. Seed Freelancer user
        var freelancerEmail = "freelancer@flow.com";
        var freelancer = await userManager.FindByEmailAsync(freelancerEmail);
        if (freelancer == null)
        {
            freelancer = new User
            {
                UserName = freelancerEmail,
                Email = freelancerEmail,
                FullName = "Sameh Mostafa",
                Role = UserRole.Freelancer,
                IsActive = true,
                MustChangePassword = false,
                CreatedAtUtc = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(freelancer, "Flow@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(freelancer, "Freelancer");
            }
        }

        // 4. Seed Clients for this Freelancer (if none exist yet)
        if (freelancer != null && !await context.Clients.AnyAsync(c => c.UserID == freelancer.Id))
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Client 1: Acme Corp
            var client1 = new Client
            {
                UserID = freelancer.Id,
                CompanyName = "Acme Global Technologies",
                Phone = "+1 (555) 019-2834",
                Address = "100 Enterprise Way, Suite 400, New York, NY",
                Notes = "Primary point of contact: John Acme (john@acme.com). Prefers bi-weekly milestone reviews.",
                IsDeleted = false
            };

            // Client 2: Nile Media Studio
            var client2 = new Client
            {
                UserID = freelancer.Id,
                CompanyName = "Nile Media Studio",
                Phone = "+20 100 123 4567",
                Address = "24 El-Tahrir St, Dokki, Giza, Egypt",
                Notes = "Creative design agency. Billing currency in EGP.",
                IsDeleted = false
            };

            // Client 3: Nordic Softworks
            var client3 = new Client
            {
                UserID = freelancer.Id,
                CompanyName = "Nordic Softworks AB",
                Phone = "+46 8 123 4567",
                Address = "Sveavägen 44, Stockholm, Sweden",
                Notes = "B2B SaaS startup. Payments processed in EUR via European wire transfer.",
                IsDeleted = false
            };

            await context.Clients.AddRangeAsync(client1, client2, client3);
            await context.SaveChangesAsync();

            // 5. Seed Client Portal User linked to Client 1 (Acme Corp)
            var clientEmail = "client@flow.com";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser == null)
            {
                clientUser = new User
                {
                    UserName = clientEmail,
                    Email = clientEmail,
                    FullName = "John Acme",
                    Role = UserRole.Client,
                    ClientId = client1.ID,
                    IsActive = true,
                    MustChangePassword = false,
                    CreatedAtUtc = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var clientRes = await userManager.CreateAsync(clientUser, "Flow@123");
                if (clientRes.Succeeded)
                {
                    await userManager.AddToRoleAsync(clientUser, "Client");
                    client1.PortalUser = clientUser;
                    context.Clients.Update(client1);
                    await context.SaveChangesAsync();
                }
            }

            // 6. Seed Projects, Milestones, Tasks, Invoices & Payments

            // Project 1 (Acme Corp): E-Commerce Cloud Migration (USD)
            var proj1 = new Project
            {
                ClientID = client1.ID,
                Name = "E-Commerce Cloud Migration",
                Description = "Migrating on-premise monolithic e-commerce application to microservices on Azure Kubernetes Service (AKS).",
                StartDate = today.AddDays(-20),
                Deadline = today.AddDays(40),
                Budget = 8500.00m,
                Currency = Currency.USD,
                Status = ProjectStatus.InProgress,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-20)
            };

            // Project 2 (Nile Media Studio): Brand Identity & Flutter App (EGP)
            var proj2 = new Project
            {
                ClientID = client2.ID,
                Name = "Brand Identity & Flutter App",
                Description = "Comprehensive brand design system and cross-platform mobile application development in Flutter.",
                StartDate = today.AddDays(-15),
                Deadline = today.AddDays(15),
                Budget = 120000.00m,
                Currency = Currency.EGP,
                Status = ProjectStatus.InProgress,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-15)
            };

            // Project 3 (Nordic Softworks): Fintech Dashboard Analytics (EUR)
            var proj3 = new Project
            {
                ClientID = client3.ID,
                Name = "Fintech Dashboard Analytics",
                Description = "High-performance real-time analytics dashboard with interactive charts and transaction monitoring.",
                StartDate = today,
                Deadline = today.AddDays(60),
                Budget = 12000.00m,
                Currency = Currency.EUR,
                Status = ProjectStatus.NotStarted,
                CreatedAtUtc = DateTime.UtcNow
            };

            await context.Projects.AddRangeAsync(proj1, proj2, proj3);
            await context.SaveChangesAsync();

            // Milestones & Tasks for Project 1 (Acme Corp)
            var m1_1 = new Milestone
            {
                ProjectID = proj1.ID,
                Name = "Phase 1: Cloud Infrastructure Setup",
                DueDate = today.AddDays(-5),
                SortOrder = 1
            };
            var m1_2 = new Milestone
            {
                ProjectID = proj1.ID,
                Name = "Phase 2: Database Migration & Sync",
                DueDate = today.AddDays(15),
                SortOrder = 2
            };
            var m1_3 = new Milestone
            {
                ProjectID = proj1.ID,
                Name = "Phase 3: Integration & Production Cutover",
                DueDate = today.AddDays(38),
                SortOrder = 3
            };

            await context.Milestones.AddRangeAsync(m1_1, m1_2, m1_3);
            await context.SaveChangesAsync();

            var t1_1 = new TaskItem
            {
                MilestoneID = m1_1.ID,
                Title = "Provision AKS cluster & ingress controller",
                Priority = TaskPriority.High,
                DueDate = today.AddDays(-8),
                Status = TaskItemStatus.Completed,
                SortOrder = 1
            };
            var t1_2 = new TaskItem
            {
                MilestoneID = m1_1.ID,
                Title = "Configure CI/CD deployment pipelines in GitHub Actions",
                Priority = TaskPriority.Medium,
                DueDate = today.AddDays(-6),
                Status = TaskItemStatus.Completed,
                SortOrder = 2
            };
            var t1_3 = new TaskItem
            {
                MilestoneID = m1_2.ID,
                Title = "Execute dry-run database migration script",
                Priority = TaskPriority.High,
                DueDate = today.AddDays(2),
                Status = TaskItemStatus.InProgress,
                SortOrder = 1
            };
            var t1_4 = new TaskItem
            {
                MilestoneID = m1_2.ID,
                Title = "Verify Redis caching and query latency benchmarks",
                Priority = TaskPriority.Medium,
                DueDate = today.AddDays(10),
                Status = TaskItemStatus.Pending,
                SortOrder = 2
            };
            var t1_5 = new TaskItem
            {
                MilestoneID = m1_3.ID,
                Title = "Stress testing checkout API under 5,000 req/sec",
                Priority = TaskPriority.Low,
                DueDate = today.AddDays(30),
                Status = TaskItemStatus.Pending,
                SortOrder = 1
            };

            await context.Tasks.AddRangeAsync(t1_1, t1_2, t1_3, t1_4, t1_5);

            // Milestones & Tasks for Project 2 (Nile Media Studio) - Includes OVERDUE tasks to test Dashboard!
            var m2_1 = new Milestone
            {
                ProjectID = proj2.ID,
                Name = "Design System & Wireframes",
                DueDate = today.AddDays(-2),
                SortOrder = 1
            };
            var m2_2 = new Milestone
            {
                ProjectID = proj2.ID,
                Name = "Core Mobile Screens & State Management",
                DueDate = today.AddDays(10),
                SortOrder = 2
            };

            await context.Milestones.AddRangeAsync(m2_1, m2_2);
            await context.SaveChangesAsync();

            var t2_1 = new TaskItem
            {
                MilestoneID = m2_1.ID,
                Title = "Finalize brand color palette and typography system",
                Priority = TaskPriority.High,
                DueDate = today.AddDays(-5),
                Status = TaskItemStatus.Completed,
                SortOrder = 1
            };
            // Overdue Task 1
            var t2_2 = new TaskItem
            {
                MilestoneID = m2_1.ID,
                Title = "Client sign-off on Figma prototypes",
                Priority = TaskPriority.High,
                DueDate = today.AddDays(-3),
                Status = TaskItemStatus.Pending, // OVERDUE!
                SortOrder = 2
            };
            // Overdue Task 2
            var t2_3 = new TaskItem
            {
                MilestoneID = m2_2.ID,
                Title = "Implement social authentication in Flutter",
                Priority = TaskPriority.Medium,
                DueDate = today.AddDays(-1),
                Status = TaskItemStatus.Pending, // OVERDUE!
                SortOrder = 1
            };
            var t2_4 = new TaskItem
            {
                MilestoneID = m2_2.ID,
                Title = "Integrate payment gateway SDK",
                Priority = TaskPriority.High,
                DueDate = today.AddDays(6),
                Status = TaskItemStatus.InProgress,
                SortOrder = 2
            };

            await context.Tasks.AddRangeAsync(t2_1, t2_2, t2_3, t2_4);
            await context.SaveChangesAsync();

            // Invoices & Payments
            // Invoice 1 for Project 1: Paid invoice
            var inv1 = new Invoice
            {
                ProjectID = proj1.ID,
                Amount = 4000.00m,
                Currency = Currency.USD,
                IssueDate = today.AddDays(-18),
                DueDate = today.AddDays(-4),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-18)
            };

            // Invoice 2 for Project 1: Unpaid invoice
            var inv2 = new Invoice
            {
                ProjectID = proj1.ID,
                Amount = 4500.00m,
                Currency = Currency.USD,
                IssueDate = today.AddDays(-2),
                DueDate = today.AddDays(14),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            };

            // Invoice 3 for Project 2: Partially Paid invoice
            var inv3 = new Invoice
            {
                ProjectID = proj2.ID,
                Amount = 60000.00m,
                Currency = Currency.EGP,
                IssueDate = today.AddDays(-14),
                DueDate = today.AddDays(7),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-14)
            };

            await context.Invoices.AddRangeAsync(inv1, inv2, inv3);
            await context.SaveChangesAsync();

            // Payment for Invoice 1 (Full payment)
            var pay1 = new Payment
            {
                InvoiceID = inv1.ID,
                Amount = 4000.00m,
                Currency = Currency.USD,
                PaymentDate = today.AddDays(-6),
                Notes = "Wire Transfer - Ref #WT-94021",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-6)
            };

            // Payment for Invoice 3 (Partial payment 25,000 / 60,000)
            var pay2 = new Payment
            {
                InvoiceID = inv3.ID,
                Amount = 25000.00m,
                Currency = Currency.EGP,
                PaymentDate = today.AddDays(-5),
                Notes = "InstaPay Transfer - Initial deposit",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
            };

            await context.Payments.AddRangeAsync(pay1, pay2);
            await context.SaveChangesAsync();
        }
    }
}
