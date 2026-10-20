using System;
using System.Collections.Generic;
using System.Text;

namespace University_Academic.Entities
{
    public class CourseSection
    {

        public int SectionId { get; set; } 
        public int CourseId { get; set; } //FK
        public Course Course { get; set; } = null!; // Navigation property
        public int InstructorId { get; set; } //FK
        public Instructor Instructor { get; set; } = null!; // Navigation property
        public int SemesterId { get; set; } //FK
        public Semester Semester { get; set; } = null!; // Navigation property
        public int ClassroomId { get; set; } //FK
        public Classroom Classroom { get; set; } = null!; // Navigation property
        public int SectionNumber { get; set; }
        public int Capacity { get; set; }
        public List<Enrollment>Enrollments { get; set; }=new List<Enrollment>();
        public List<SectionSchedule> SectionSchedules { get; set; } = new();

    }
}
