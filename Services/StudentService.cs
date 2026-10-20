
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;

namespace University_Academic.Services
{
    public class StudentService
    {
        private readonly UniversityDbContext _context;

        public StudentService(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult AddStudent(string fullName, string email, string? phone
            , DateTime dateOfBirth, Gender gender, AcademicLevel level, DateTime enrollmentDate, int departmentId)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Failure("Invalid email address format.");

            var IsStudentExist = _context.Students.FirstOrDefault(e => e.Email == email);
            if (IsStudentExist != null)
            {
                return Failure("Student & Email Is already Exist ");
            }
            if(DateTime.Now.Year - dateOfBirth.Year <= 16)
            {
                return Failure("Student must be older than  16 year ");
            }
            if(!_context.Departments.Any(e=>e.DepartmentId == departmentId))
            {
                return Failure("Department ID Not Exist ");
            }
            string UniversityId = GenerateNextUniversityId();
            var student = new Student
            {
                UniversityId= UniversityId,
                FullName = fullName,
                Email = email,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                AcademicLevel = level,
                EnrollmentDate = enrollmentDate,
                DepartmentId = departmentId
            };

            _context.Students.Add(student);
            _context.SaveChanges();
            return Success();
        }
        string GenerateNextUniversityId()
        {
            var lastStudent= _context.Students.OrderBy(e=>e.StudentId).LastOrDefault();
            if(lastStudent == null )
            {
                return "ST0001";
            }
            string numericPart = lastStudent.UniversityId.Replace("ST", "");
            if(int.TryParse(numericPart,out int lastNumber))
            {
                lastNumber += 1;
                return $"ST{lastNumber:D4}";
            }
            return "ST0001";
        }

        public List<Student> GetStudents()
        {
            return _context.Students
                .Include(e=>e.Enrollments)
                .Include(e=>e.Department)
                .ToList();
        }
        public Student? GetStudentById(int id)
        {
            return _context.Students
                .Include(e => e.Department)
                
                .FirstOrDefault(e=>e.StudentId == id);
        }
        public Student? GetStudentById(string universityId)
        {
            return _context.Students.FirstOrDefault(e=>e.UniversityId == universityId);
        }
        public List<Student> SearchStudents(string query)
        {
            var upperQuery = query.ToUpper();
            return _context.Students
                .Include(e=>e.Department)
                .Include(e=>e.Enrollments)
                .Where(e => e.FullName.Contains(query)
                       || e.UniversityId.Contains(upperQuery)).ToList();
        }
        public ServiceResult UpdateStudent(int id, string fullName, string email, string? phone
            , DateTime dateOfBirth, Gender gender, AcademicLevel level, int departmentId)
        {
            var student = _context.Students.FirstOrDefault(e=>e.StudentId==id);
            if(student == null)
            {
                return Failure("Student Id Not Exist ");
            }

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Failure("Invalid email address format.");

            if (DateTime.Now.Year - dateOfBirth.Year <= 16)
            {
                return Failure("Student must be older than  16 year ");
            }
            if (!_context.Departments.Any(e => e.DepartmentId == departmentId))
            {
                return Failure("Department ID Not Exist ");
            }
            student.FullName = fullName;
            student.Email = email;
            student.Phone = phone;
            student.DateOfBirth = dateOfBirth;
            student .Gender = gender;
            student.AcademicLevel = level;
            student.DepartmentId = departmentId;

            _context.SaveChanges();

            return Success();
        }
        public List<Enrollment>GetStudentEnrollment(int studentId)
        {
            return _context.Enrollments.Where(e=>e.StudentId == studentId)
                .Include(e=>e.Section)
                .ThenInclude(e=>e.Course)
                .ToList();
        }
        public ServiceResult SuspendStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(e => e.StudentId == id);
            if (student == null)
            {
                return Failure("Student Id Not Exist ");
            }
            if(student.Status == StudentStatus.Suspended)
            {
                return Failure("Student Is Already Suspended");
            }
            student.Status = StudentStatus.Suspended;
            _context.SaveChanges();

            return Success();
        }
        public ServiceResult ActivateStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(e => e.StudentId == id);
            if (student == null)
            {
                return Failure("Student Id Not Exist ");
            }
            if (student.Status == StudentStatus.Active)
            {
                return Failure("Student Is Already Active");
            }
            student.Status = StudentStatus.Active;
            _context.SaveChanges();

            return Success();
        }
        public ServiceResult WithdrawStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(e => e.StudentId == id);
            if (student == null)
            {
                return Failure("Student Id Not Exist ");
            }
            if (student.Status == StudentStatus.Withdrawn)
            {
                return Failure("Student Is Already Withdrawn");
            }
            student.Status = StudentStatus.Withdrawn;
            _context.SaveChanges();

            return Success();
        }
    }
}
