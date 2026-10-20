using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Entities;

namespace University_Academic.Data
{
    public class UniversityDbContext:DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options):base(options)
        {
        }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<CoursePrerequisite>CoursePrerequisites { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<SectionSchedule> SectionSchedules { get; set; }
        public DbSet<Grade> Grades { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);
        }
       
    }
}
