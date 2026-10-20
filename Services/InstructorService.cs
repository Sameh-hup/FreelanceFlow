
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;

namespace University_Academic.Services
{
    public class InstructorService
    {
        private readonly UniversityDbContext _context;

        public InstructorService(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult AddInstructor(string fullName, string email, string? phone, AcademicTitle academicTitle, DateTime hireDate,int departmentid)
        {
            var IsExist = _context.Instructors.FirstOrDefault(e => e.Email == email);
            if (IsExist != null)
            {
                return Failure("Instructor & Email Is already Exist ");
            }
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Failure("Invalid email address format.");
                
            if (hireDate > DateTime.Now)
            {
                return Failure("Error: Hire date must be in the past.");
            }
            bool departmentExist = _context.Departments.Any(e=>e.DepartmentId == departmentid);
            if (!departmentExist)
            {
                return Failure("Department Not Exist ");
            }
            var instructor = new Instructor
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                AcademicTitle = academicTitle,
                HireDate = hireDate,
                DepartmentId = departmentid
            };
            _context.Instructors.Add(instructor);
            _context.SaveChanges();
            return Success();
        }
        public List<Instructor> GetAllInstructors()
        {
            return _context.Instructors.ToList();
        }
        public Instructor? GetInstructor(int id)
        {
            return _context.Instructors.FirstOrDefault(e => e.InstructorId == id);
        }
        public List<Instructor>? GetInstructorsByDepartment(int departmentId)
        {
            return _context.Instructors.Where(e => e.DepartmentId == departmentId).ToList();
        }
        public ServiceResult UpdateInstructor( int id, string fullName, string? phone, AcademicTitle title, int departmentId)
        {
            var instructor = _context.Instructors.FirstOrDefault(e => e.InstructorId == id);
            if(instructor == null)
            {
                return Failure("Can't Find Instructor ");
            }
            bool departmentExist = _context.Departments.Any(e => e.DepartmentId == departmentId);
            if (!departmentExist)
            {
                return Failure("Department Not Exist ");
            }
            instructor.FullName = fullName;
            instructor .Phone = phone;
            instructor.AcademicTitle = title;
            instructor.DepartmentId = departmentId;
            _context.SaveChanges();

            return Success();
        }
        public ServiceResult ResignInstructor(int id)
        {
            var instructor = GetInstructor(id);
            if(instructor == null)
            {
                return Failure("Can't found Instructor");
            }
            bool hasActiveSections = _context.CourseSections
                .Any(s => s.InstructorId == id && s.Semester.Status == SemesterStatus.Active);
            if (hasActiveSections)
            {
                return Failure($"Con't Resign : {instructor.FullName} ,because he has an active Sections ");
            }
            instructor.Status = InstructorStatus.Resigned;
            _context.SaveChanges();
            return Success();
        }
        public List<CourseSection>? GetSectionsByInstructor(int id)
        {
            return _context.CourseSections.Where(e => e.InstructorId == id).Include(e=>e.SectionSchedules).ToList();
        }
        public int GetInstructorTeachingLoad(int instructorId,int semesterId)
        {
            return _context.CourseSections.Count(e => e.InstructorId == instructorId && e.SemesterId == semesterId);
        }
    }
}
