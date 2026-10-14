using University_Academic_System.Entities;
using University_Academic_System.Data;
using Microsoft.EntityFrameworkCore;
namespace University_Academic_System.Services;

public class CollegeService
{
    private readonly UniversityDbContext _context;

    public CollegeService(UniversityDbContext context)
    {
        _context = context;
    }
    public void AddCollege(string name, string code, string description)
    {
        var college = new College
        {
            Name = name,
            Code = code,
            Description = description
        };
        _context.College.Add(college);
        _context.SaveChanges();
    }

    public List<College> GetAllColleges()
    {
        return  _context.College.ToList();
        
    }

    public College? GetCollegeById(int id)
    {
        var college = _context.College.FirstOrDefault(e => e.CollegeId == id);
        return college;
    }

    public List<College> SearchColleges(string term)
    {
        var list = _context.College
            .Where(e => e.Name.Contains(term) || e.Code.Contains(term)).ToList();
        return list;
    }

    public bool UpdateCollege(int id, string name, string code, string description)
    {
        var college = _context.College.FirstOrDefault(e => e.CollegeId == id);

        if (college == null) return false;

        college.Name = name;
        college.Code = code;
        college.Description = description;

        _context.SaveChanges();
        return true;
    }

    public bool DeleteCollege(int id)
    {
        var college = _context.College
            .Include(e => e.Departments)
            .FirstOrDefault(e => e.CollegeId == id);

        if (college == null) return false;

        if (college.Departments != null && college.Departments.Any())
        {
            return false;
        }
        _context.College.Remove(college);
        _context.SaveChanges();
        return true;
    }

    public List<Department>  GetDepartmentsByCollege(int collegeid)
    {
        //var college = _context.College
        //    .Include (e => e.Departments)
        //    .FirstOrDefault(e => e.CollegeId == collegeid);

        //if (college == null || college.Departments == null)
        //{
        //    return new List<Department>();
        //}
        //return college.Departments.ToList();

        return _context.Department
            .Where(e => e.CollegeId == collegeid)
            .ToList();
    }
}
