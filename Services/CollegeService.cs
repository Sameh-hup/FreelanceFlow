using University_Academic.Entities;
using University_Academic.Data;
using Microsoft.EntityFrameworkCore;
using University_Academic.Helper_Function;
using System.Diagnostics.Contracts;
namespace University_Academic.Services;
using static University_Academic.Helper_Function.ServiceResult;

public class CollegeService
{
    private readonly UniversityDbContext _context;

    public CollegeService(UniversityDbContext context)
    {
        _context = context;
    }
    public ServiceResult AddCollege(string name, string code, string description)
    {
        
        code = code.Trim().ToUpper();
        bool isCodeExists = _context.Colleges.Any(e => e.Code == code);
        if (isCodeExists)
        {
            return Failure("College code already exists.");
        }
        if (code.Length < 2 || code.Length > 10 || code.Contains(" "))
            return Failure("College code must be between 2 and 10 characters and contain no spaces.");
        var college = new College
        {
            Name = name,
            Code = code,
            Description = description
        };
        _context.Colleges.Add(college);
        _context.SaveChanges();
        return Success();
    }

    public List<College> GetAllColleges()
    {
        return  _context.Colleges.ToList();
        
    }

    public ServiceResult<College> GetCollegeById(int id)
    {
        var college = _context.Colleges.FirstOrDefault(e => e.CollegeId == id);
        if(college == null)
        {
            return ServiceResult<College>.Failure("College not found.");
        }
        return ServiceResult<College>.Success(college);
    }

    public List<College> SearchColleges(string term)
    {
        var list = _context.Colleges
            .Where(e => e.Name.Contains(term) || e.Code.Contains(term)).ToList();
        return list;
    }

    public ServiceResult UpdateCollege(int id, string name, string code, string description)
    {
        var college = _context.Colleges.FirstOrDefault(e => e.CollegeId == id);

        if (college == null) return Failure("College not found.");
        code = code.Trim().ToUpper();
        if (code.Length < 2 || code.Length > 10 || code.Contains(" "))
            return Failure("College code must be between 2 and 10 characters and contain no spaces.");
        college.Name = name;
        college.Code = code;
        college.Description = description;

        _context.SaveChanges();
        return Success();
    }

    public ServiceResult DeleteCollege(int id)
    {
        var college = _context.Colleges
            .Include(e => e.Departments)
            .FirstOrDefault(e => e.CollegeId == id);

        if (college == null) return Failure("College not found.");

        if (college.Departments != null && college.Departments.Any())
        {
            return Failure("Cannot delete college with associated departments.");
        }
        _context.Colleges.Remove(college);
        _context.SaveChanges();
        return Success();
    }

    public List<Department>  GetDepartmentsByCollege(int collegeId)
    {
        //var college = _context.College
        //    .Include (e => e.Departments)
        //    .FirstOrDefault(e => e.CollegeId == collegeId);

        //if (college == null || college.Departments == null)
        //{
        //    return new List<Department>();
        //}
        //return college.Departments.ToList();

        return _context.Departments
            .Where(e => e.CollegeId == collegeId)
            .ToList();
    }
}
