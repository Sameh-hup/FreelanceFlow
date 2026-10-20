using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Entities;
using University_Academic.Enums;

namespace University_Academic.Configurations
{
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(e => e.UniversityId)
                .IsRequired()
                .HasMaxLength(15);
            builder.HasIndex(e => e.UniversityId).IsUnique();
            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Phone).HasMaxLength(20);
            builder.Property(e => e.DateOfBirth).IsRequired();
            builder.Property(e => e.Gender).IsRequired();
            builder.Property(e => e.AcademicLevel).IsRequired();
            builder.Property(e => e.EnrollmentDate).IsRequired();
            builder.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue(StudentStatus.Active);

            builder.ToTable(t => 
            {
                
                t.HasCheckConstraint("CK_Student_Gender", "[Gender] IN (1,2)");
                t.HasCheckConstraint("CK_Student_AcademicLevel", "[AcademicLevel] IN (1,2,3,4)");
                t.HasCheckConstraint("CK_Student_EnrollmentDate", "[EnrollmentDate] <= GETDATE()");
                t.HasCheckConstraint("CK_Student_Status", "[Status] IN (1,2,3,4)");

            });


            builder.HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


           
        }
    }
}
