
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;
namespace University_Academic.Services;

public class SemesterService
{
    private readonly UniversityDbContext _context;

    public SemesterService(UniversityDbContext context)
    {
        _context = context;
    }
    // comment all method.



    //Add a new semester to the database.
    
    public ServiceResult AddSemester(SemesterName name,string academicYear,DateTime startDate ,DateTime endDate)
    {
        var existingSemester = _context.Semesters.FirstOrDefault(s => s.Name == name && s.AcademicYear == academicYear);                                 
        if(existingSemester != null)
        {
            return Failure("Semester with the same name and academic year already exists.");
        }
        if (startDate >= endDate)return Failure("Start date must be before end date.");

        var semester = new Semester
        {
            Name = name,
            AcademicYear = academicYear,
            StartDate = startDate,
            EndDate = endDate,
            
        };
        _context.Semesters.Add(semester);
        _context.SaveChanges();
        return Success();
    }
    public List<Semester> GetAllSemesters()
    {
        return _context.Semesters.ToList();
    }
    // Activate a semester by its ID. Only one semester can be active at a time.
    public ServiceResult ActivateSemester(int semesterId)
    {
        var semester = _context.Semesters.FirstOrDefault(s => s.SemesterId == semesterId);
        if (semester == null) return Failure("Semester not found.");

        if(semester.Status == SemesterStatus.Active)return Failure("Semester is already active.");

        var activeSemester = _context.Semesters.FirstOrDefault(s => s.Status == SemesterStatus.Active);
        if(activeSemester != null )
        {
            return Failure("Another semester is already active. Please complete it before activating a new semester.");
        }

        if (semester.Status== SemesterStatus.Completed) return Failure("Cannot activate a completed semester.");

        semester.Status = SemesterStatus.Active;
        _context.SaveChanges();
        return Success();
    }
    // Complete a semester by its ID. Only active semesters can be completed.
    public ServiceResult CompleteSemester(int semesterId)
    {
        var semester =_context.Semesters.FirstOrDefault(s => s.SemesterId == semesterId);
        if (semester == null) return Failure("Semester not found.");
        if(semester.Status == SemesterStatus.Completed) return Failure("Semester is already completed.");
        if(semester.Status != SemesterStatus.Active)return Failure("Only active semesters can be completed.");

        semester.Status = SemesterStatus.Completed;
        _context.SaveChanges();
        return Success();
    }
}
