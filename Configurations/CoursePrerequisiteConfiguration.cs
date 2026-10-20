using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Entities;

namespace University_Academic.Configurations
{
    public class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
    {
        public void Configure(EntityTypeBuilder<CoursePrerequisite> builder)
        {
            builder.HasKey(c => new {c.CourseId,c.PrerequisiteCourseId});
            builder.ToTable(e => e.HasCheckConstraint(
                "CK_CoursePrerequisite_CourseId != PrerequisiteCourseId",
                "[PrerequisiteCourseId] <> [CourseId]"));
            builder.HasOne(c => c.Course)
                .WithMany(e => e.Prerequisites)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.PrerequisiteCourse)
                .WithMany(d => d.InPrerequisitesListOf)
                .HasForeignKey(e => e.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
