using System.Diagnostics.CodeAnalysis;
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string UniversityId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public AcademicLevel AcademicLevel { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
