using System;
using University_Academic_System.Enums;

namespace University_Academic_System.Entities
{
    public class Semester
    {
        public int Id { get; set; }
        public SemesterName Name { get; set; }
        public int AcademicYear { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SemesterStatus Status { get; set; }
    }
}