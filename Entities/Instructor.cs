using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;  
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public AcademicTitle AcademicTitle { get; set; }
        public DateTime HireDate { get; set; }
        public InstructorStatus Status { get; set; } = InstructorStatus.Active;
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public List<Student> AdvisedStudents { get; set; } = new();
        public List<CourseSection> CourseSections { get; set; } = new();


    }
}
