
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace University_Academic.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [NotNull]
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } =string.Empty;
        public string? Description { get; set; }
        public int CollegeId { get; set; }
        public College? College { get; set; }



        public int? ChairPersonId { get; set; }
        public Instructor? ChairPerson { get; set; }
        public List<Course> Courses { get; set; } = new();
        public List<Instructor> Instructors { get; set; } = new();
        public List<Student> Students { get; set; } = new();
    }
}
