using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using University_Academic_System.Enums;

namespace University_Academic_System.Entities;

public class Instructor
{
    [Key]
    public int InstructorId { get; set; }

    [NotNull]
    public string FullName { get; set; } = string.Empty;

    [NotNull]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public AcademicTitle AcademicTitle { get; set; }

    public DateTime HireDate { get; set; }

    public InstructorStatus Status { get; set; } = InstructorStatus.Active;

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}