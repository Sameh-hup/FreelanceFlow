using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
    {
        public void Configure(EntityTypeBuilder<CourseSection> builder)
        {
            builder.HasKey(e => e.SectionId);
            builder.Property(e => e.CourseId).IsRequired();
            builder.Property(e => e.InstructorId).IsRequired();
            builder.Property(e => e.SemesterId).IsRequired();
            builder.Property(e => e.ClassroomId).IsRequired();

            builder.Property(e => e.SectionNumber).IsRequired();
            builder.HasIndex(e => new { e.SectionNumber, e.CourseId, e.SemesterId }).IsUnique();
            builder.Property(e => e.Capacity).IsRequired();

            builder.ToTable(e => e.HasCheckConstraint("CK_CourseSection_Capacity", "[Capacity] > 0"));
            //Course ---- CourseSection
            builder.HasOne(e => e.Course)
                .WithMany(e => e.CourseSections)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            //Instructor ---- CourseSection
            builder.HasOne(e => e.Instructor)
                .WithMany(e => e.CourseSections)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
            //Semester ---- CourseSection
            builder.HasOne(e => e.Semester)
                .WithMany(e => e.CourseSections)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);
            //ClassroomId ---- CourseSection
            builder.HasOne(e => e.Classroom)
                .WithMany(e => e.CourseSections)
                .HasForeignKey(e => e.ClassroomId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
