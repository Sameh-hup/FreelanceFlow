using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic_System.Entities;

namespace University_Academic_System.Data
{
    public class UniversityDbContext:DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options):base(options)
        {
        }
        public DbSet<College> College { get; set; }
        public DbSet<Department> Department { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);
        }
       
    }
}
