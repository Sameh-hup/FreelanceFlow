
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace University_Academic_System.Entities;

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



   
}
