using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;
namespace University_Academic.Services
{
    public class GradeServices
    {
        private readonly UniversityDbContext _context;
        public GradeServices(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult RecordGrade(int enrollmentId, decimal midterm, decimal practical, decimal finalScore)
        {
            var enrollment = _context.Enrollments
                .Include(e=>e.Grade)
                .Include(e=>e.Section).ThenInclude(e=>e.Semester)
                .FirstOrDefault(e=>e.EnrollmentId == enrollmentId);
            if(enrollment == null)
            {
                return Failure("Enrollment Id Not Exist ");
            }
            if(enrollment.Section.Semester.Status != SemesterStatus.Active
                &&enrollment.Section.Semester.Status != SemesterStatus.Completed )
            {
                return Failure("Recording grades is permitted only in an 'Active' or 'Completed' term.");
            }
            if(enrollment.Grade != null)
            {
                return Failure("This Enrollment has been Recorded before ");
            }
            if (midterm > 20 || midterm < 0){ return Failure("Grad of Midterm must be in (20.00 - 0.00)"); }
            if (practical > 20 || practical < 0) { return Failure("Grad of Practical must be in (20.00 - 0.00)"); }  
            if (finalScore > 60 || finalScore < 0) { return Failure("Grad of FinalScore must be in (60.00 - 0.00)"); }

            var grade = new Grade
            {
                EnrollmentId = enrollmentId,
                Midterm = midterm,
                Practical = practical,
                Final = finalScore
            };

            _context.Grades.Add(grade);
            enrollment.Status=EnrollmentStatus.Completed;
            _context.SaveChanges();
            return Success($"Grade Record successfully! New total: {grade.Total} | Letter grade: {grade.LetterGrade} | Grade points: {grade.GradePoint:F2}.");
        }

        public ServiceResult UpdateGrade(int gradeId, decimal midterm, decimal practical, decimal finalScore)
        {
            var grade = _context.Grades
               .Include(e => e.Enrollment).ThenInclude(e => e.Section).ThenInclude(e => e.Semester)
                .FirstOrDefault(e=>e.GradeId == gradeId);
            if(grade == null)
            {
                return Failure("Grade Not Exist ");
            }
            if (grade.Enrollment.Section.Semester.Status != SemesterStatus.Active
                && grade.Enrollment.Section.Semester.Status != SemesterStatus.Completed)
            {
                return Failure("Recording grades is permitted only in an 'Active' or 'Completed' term.");
            }
            if (midterm > 20 || midterm < 0) { return Failure("Grad of Midterm must be in (20.00 - 0.00)"); }
            if (practical > 20 || practical < 0) { return Failure("Grad of Practical must be in (20.00 - 0.00)"); }
            if (finalScore > 60 || finalScore < 0) { return Failure("Grad of FinalScore must be in (60.00 - 0.00)"); }

            grade.Midterm = midterm;
            grade.Practical = practical;
            grade.Final = finalScore;
            _context.SaveChanges();
            return ServiceResult.Success($"Grade updated successfully! New total: {grade.Total} | Letter grade: {grade.LetterGrade} | Grade points: {grade.GradePoint:F2}.");
            
        }

        public Grade? GetGradeByEnrollment(int enrollmentId)
        {
            return _context.Grades
                .Include(e=>e.Enrollment).ThenInclude(e=>e.Student)
                .Include(e=>e.Enrollment).ThenInclude(e=>e.Section).ThenInclude(e=>e.Course)
                .FirstOrDefault(e=>e.EnrollmentId == enrollmentId);
        }
        public List<Grade> GetGradesBySection(int sectionId)
        {
            return _context.Grades
                .Include(e => e.Enrollment).ThenInclude(e => e.Student)
                .Include(e => e.Enrollment).ThenInclude(e => e.Section).ThenInclude(e => e.Course)
                .Where(e => e.Enrollment.SectionId == sectionId)
                .OrderBy(e => e.Enrollment.Student.FullName)
                .ToList();
        }
        public List<Grade> GetAllGrades ()
        {
            return _context.Grades
                .Include(e => e.Enrollment).ThenInclude(e => e.Student)
                .Include(e => e.Enrollment).ThenInclude(e => e.Section).ThenInclude(e => e.Course)
                .ToList();
        }
        public List<Enrollment> GetAllEnrollments() 
        {
            return _context.Enrollments
                .Include(e=>e.Student)
                .Include(e=>e.Grade)
                .Include(e=>e.Section).ThenInclude(e => e.Course)
                .ToList();
        }
        public List<Student> GetAllStudent() 
        {
            return _context.Students.Include(e => e.Department).ToList();      
        }
        public List<Semester> GetAllSemesters() 
        {
            return _context.Semesters.ToList();      
        }
        public List<CourseSection> GetAllSection()  
        {
            return _context.CourseSections
                .Include(e=>e.Course)
                .ToList();
        }
    }
}
