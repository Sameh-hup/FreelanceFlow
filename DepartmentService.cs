using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;

namespace University_Academic.Services;

public class DepartmentService
{
    private readonly UniversityDbContext _context;

    public DepartmentService(UniversityDbContext context)
    {
        _context = context;
    }

    public void AddDepartment(string name, string code, string description, int collegeid)
    {
        var department = new Department
        {
            Name = name,
            Code = code,
            Description = description,
            CollegeId = collegeid
        };
        _context.Department.Add(department);
        _context.SaveChanges();
    }
    public List<Department> GetAllDepartments()
    {
        return _context.Department.ToList();
    }
    public Department? GetDepartmentById(int departmentId)
    {
        return _context.Department.FirstOrDefault(e => e.DepartmentId == departmentId);
    }
    public List<Department> SearchDepartments(string term)
    {
        return _context.Department
            .Where(e => e.Name.Contains(term) || e.Code.Contains(term)).ToList();
    }
    public bool UpdateDepartment(int id, string name, string code, string description, int collegeid)
    {
        var department = _context.Department.FirstOrDefault(e => e.DepartmentId == id);
        if (department == null) return false;
        department.Name = name;
        department.Code = code;
        department.Description = description;
        department.CollegeId = collegeid;

        _context.SaveChanges();
        return true;
    }
    public bool DeleteDepartment(int departmentId)
    {
        var department = _context.Department
            .Include(e => e.Students)
            .FirstOrDefault(e => e.DepartmentId == departmentId);
        if (department == null) return false;
        if (department.Students != null && department.Students.Any()) return false;
        _context.Department.Remove(department);
        _context.SaveChanges();
        return true;
    }
}
