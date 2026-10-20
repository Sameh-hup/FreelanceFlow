using System.Diagnostics.CodeAnalysis;
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; } 
        public int CreditHours { get; set; }
        public AcademicLevel Level { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public List<CoursePrerequisite> Prerequisites { get; set; } = [];
        public List<CoursePrerequisite> InPrerequisitesListOf { get; set; } = [];

        public List<CourseSection> CourseSections { get; set; } = [];
        

    }
}
