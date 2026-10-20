using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public SemesterName Name { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SemesterStatus Status { get; set; } = SemesterStatus.Upcoming;
        public List<CourseSection> CourseSections { get; set; } = new();

    }
}
