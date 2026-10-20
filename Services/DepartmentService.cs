using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using static University_Academic.Helper_Function.ServiceResult;
using University_Academic.Helper_Function;
namespace University_Academic.Services;

public class DepartmentService
{
    private readonly UniversityDbContext _context;

    public DepartmentService(UniversityDbContext context)
    {
        _context = context;
    }

    public ServiceResult AddDepartment(string name, string code, string description, int collegeId)
    {
        bool collegeExists = _context.Colleges.Any(c => c.CollegeId == collegeId );
        if (!collegeExists)
        {
            return Failure($"College with ID {collegeId} does not exist.");
        }
        code = code.Trim().ToUpper();
        bool departmentExists = _context.Departments.Any(d => d.Code == code);
        if (departmentExists)
        {
            return Failure($"Department with code {code} already exists .");
        }
        if (_context.Departments.Any(d => d.Name == name && d.CollegeId == collegeId))
            return Failure($"Department with name '{name}' already exists in this college.");

        if (code.Length < 2 || code.Length > 10 || code.Contains(" "))
            return Failure("Department code must be between 2 and 10 characters and contain no spaces.");
        var department = new Department
        {
            Name = name,
            Code = code,
            Description = description,
            CollegeId = collegeId
        };
        _context.Departments.Add(department);
        _context.SaveChanges();
        return Success();
    }
    public List<Department> GetAllDepartments()
    {
        return _context.Departments.ToList();
    }
    public ServiceResult<Department> GetDepartmentById(int departmentId)
    {
        var department = _context.Departments.FirstOrDefault(e => e.DepartmentId == departmentId);
        if (department == null)
        {
            return ServiceResult<Department>.Failure("Department not found.");
        }
        return ServiceResult<Department>.Success(department);
    }
    public List<Department> SearchDepartments(string term)
    {
        return _context.Departments
            .Where(e => e.Name.Contains(term) || e.Code.Contains(term)).ToList();
    }
    public ServiceResult UpdateDepartment(int id, string name, string code, string description, int collegeid)
    {
        var department = _context.Departments.FirstOrDefault(e => e.DepartmentId == id);
        if (department == null) return Failure("Department not found.");

        code = code.Trim().ToUpper();
        if (code.Length < 2 || code.Length > 10 || code.Contains(" "))
            return Failure("Department code must be between 2 and 10 characters and contain no spaces.");

        department.Name = name;
        department.Code = code;
        department.Description = description;
        department.CollegeId = collegeid;

        _context.SaveChanges();
        return Success();
    }
    public ServiceResult DeleteDepartment(int departmentId)
    {
        var department = _context.Departments
            .Include(e => e.Students)
            .FirstOrDefault(e => e.DepartmentId == departmentId);
        if (department == null) return Failure("Department not found.");

        bool hasRelations = _context.Students.Any(s => s.DepartmentId == departmentId)
                 || _context.Courses.Any(c => c.DepartmentId == departmentId)
                 || _context.Instructors.Any(i => i.DepartmentId == departmentId);

        if (hasRelations) 
            return Failure("Cannot delete department with associated students or courses or instructor");
        _context.Departments.Remove(department);
        _context.SaveChanges();
        return Success();
    }
    public  List<Course>GetDepartmentCourses(int departmentId)
    {
        return _context.Courses
            .Where(e => e.DepartmentId == departmentId)
            .ToList();
    }
    public List<Student> GetDepartmentStudent(int departmentId)
    {
        return _context.Students
            .Where(e=>e.DepartmentId == departmentId)
            .ToList();
    }
    public List<Instructor> GetDepartmentInstructor(int departmentId)
    {
        return _context.Instructors
            .Where(e=>e.DepartmentId == departmentId)
            .ToList();
    }
    public List<College> GetAvailableCollege()
    {
        return _context.Colleges
            .Select(c => new College { CollegeId = c.CollegeId, Name = c.Name, Code = c.Code })
            .ToList();
    }
    public List<Department> GetAvailableDepartment()
    {
        return _context.Departments
            .Select(c => new Department { DepartmentId = c.DepartmentId, Name = c.Name })
            .ToList();
    }
}
