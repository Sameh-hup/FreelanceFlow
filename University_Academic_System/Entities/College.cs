using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace University_Academic_System.Entities;
public class College
{
    public int CollegeId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; } 
    public List<Department> Departments { get; set; } = new();
}
