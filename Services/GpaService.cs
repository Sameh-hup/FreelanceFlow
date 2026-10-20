using Microsoft.EntityFrameworkCore;

using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult<decimal>;

namespace University_Academic.Services
{
    public class GpaService
    {
        private readonly UniversityDbContext _context;

        public GpaService(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult< decimal> CalculateSemesterGpa(int studentId, int semesterId)
        {
            var semesterEnrollment = _context.Enrollments
                .Include(e=>e.Grade)
                .Include(e=>e.Section).ThenInclude(e=>e.Course)
                .Where(e => e.StudentId == studentId
                && e.Section.SemesterId == semesterId
                && e.Status == EnrollmentStatus.Completed 
                && e.Grade != null).ToList();

            if (!semesterEnrollment.Any())
            {
                return Failure("The student has not registered for any semester yet.");

            }
            decimal totalQualityPoints = semesterEnrollment
            .Sum(e => e.Grade!.GradePoint * e.Section.Course.CreditHours);

            int totalHours = semesterEnrollment.Sum(e=>(int)e.Section.Course.CreditHours);

            if (totalHours == 0)
                return Failure("Student Has 0 Credit Hours");

            decimal result = Math.Round(totalQualityPoints / totalHours, 2);
            return Success(result) ;

        }

        public decimal CalculateCgpa(int studentId)
        {
            var completedEnrollments = _context.Enrollments
               .Include(e => e.Grade)
               .Include(e => e.Section).ThenInclude(e => e.Course)
               .Include(e => e.Section).ThenInclude(e => e.Semester)
               .Where(e => e.StudentId == studentId
                        && e.Status == EnrollmentStatus.Completed
                        && e.Grade != null).ToList();

            if (!completedEnrollments.Any()) return 0.00m;
               
            var latestAttempts = completedEnrollments
            .GroupBy(e => e.Section.CourseId)
            .Select(group => group.OrderByDescending(e => e.Section.Semester.StartDate)
                        .ThenByDescending(e => e.RegistrationDate)
                        .First()).ToList();
            
            decimal totalQualityPoints = latestAttempts
                .Sum(e => e.Grade!.GradePoint * e.Section.Course.CreditHours);
            int totalHours = latestAttempts
                .Sum(e => (int)e.Section.Course.CreditHours);
            if (totalHours == 0)
                return 0.00m;
            return Math.Round(totalQualityPoints / totalHours, 2);
        }

        public int CalculateEarnedHours(int studentId)
        {
            var completedEnrollments = _context.Enrollments
                .Include(e => e.Grade)
                .Include(e => e.Section).ThenInclude(s => s.Course)
                .Include(e => e.Section).ThenInclude(s => s.Semester)
                .Where(e => e.StudentId == studentId
                         && e.Status == EnrollmentStatus.Completed
                         && e.Grade != null).ToList();
                
            if (!completedEnrollments.Any())
                return 0;
            var latestAttempts = completedEnrollments
                .GroupBy(e => e.Section.CourseId)
                .Select(group => group.OrderByDescending(e => e.Section.Semester.StartDate)
                                     .ThenByDescending(e => e.RegistrationDate)
                                     .First()) .ToList();
               
            
            return latestAttempts
                .Where(e => e.Grade!.Total >= 60.00m)
                .Sum(e => (int)e.Section.Course.CreditHours);
        }

        public AcademicStanding GetAcademicStanding(int studentId)
        {
            decimal cgpa = CalculateCgpa(studentId);
            return cgpa switch
            {
                >= 3.50m => AcademicStanding.Excellent,
                >= 3.00m => AcademicStanding.VeryGood,
                >= 2.50m => AcademicStanding.Good,
                >= 2.00m => AcademicStanding.Pass,
                _ => AcademicStanding.Probation
            };
        }
        public bool CheckGraduationEligibility(int studentId)
        {
            int earnedHours = CalculateEarnedHours(studentId);
            decimal cgpa = CalculateCgpa(studentId);
            return earnedHours >= 120 && cgpa >= 2.00m;
        }
        public StudentTranscriptDto GetStudentTranscript(int studentId)
        {
            var student = _context.Students
                .Include(s => s.Department)
                .FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
                return new StudentTranscriptDto();
            var enrollments = _context.Enrollments
                .Include(e => e.Grade)
                .Include(e => e.Section).ThenInclude(s => s.Course)
                .Include(e => e.Section).ThenInclude(s => s.Semester)
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed && e.Grade != null)
                .OrderBy(e => e.Section.Semester.StartDate)
                .ToList();
            var transcript = new StudentTranscriptDto
            {
                UniversityId = student.UniversityId,
                StudentName = student.FullName,
                DepartmentName = student.Department.Name,
                CumulativeGpa = CalculateCgpa(studentId),
                TotalEarnedHours = CalculateEarnedHours(studentId),
                Standing = GetAcademicStanding(studentId),
                IsEligibleForGraduation = CheckGraduationEligibility(studentId)
            };
            
            var semesterGroups = enrollments
                .GroupBy(e => new { e.Section.SemesterId, e.Section.Semester.Name, e.Section.Semester.AcademicYear })
                .OrderBy(g => g.First().Section.Semester.StartDate);

            foreach (var semGroup in semesterGroups)
            {
                var res = CalculateSemesterGpa(studentId, semGroup.Key.SemesterId);
                var semRecord = new SemesterRecordDto
                {
                    SemesterName = semGroup.Key.Name.ToString(),
                    AcademicYear = semGroup.Key.AcademicYear,
                    SemesterGpa = res.IsSuccess ? res.Data : 0.00m
                };
                foreach (var item in semGroup)
                {
                    semRecord.Courses.Add(new CourseGradeDto
                    {
                        CourseCode = item.Section.Course.Code,
                        CourseName = item.Section.Course.Name,
                        CreditHours = item.Section.Course.CreditHours,
                        TotalScore = item.Grade!.Total,
                        LetterGrade = item.Grade.LetterGrade,
                        GradePoint = item.Grade.GradePoint,
                        IsPassed = item.Grade.Total >= 60.00m
                    });
                }
                transcript.SemesterRecords.Add(semRecord);
            }
            return transcript;
        }
        public Student GetStudent(int id)
        {

            return _context.Students.FirstOrDefault(e => e.StudentId == id) ?? new Student();


        }
    }

    public class StudentTranscriptDto
    {
        public string UniversityId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public decimal CumulativeGpa { get; set; }
        public int TotalEarnedHours { get; set; }
        public AcademicStanding Standing { get; set; }
        public bool IsEligibleForGraduation { get; set; }
        public List<SemesterRecordDto> SemesterRecords { get; set; } = new();
    }

    public class SemesterRecordDto
    {
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public decimal SemesterGpa { get; set; }
        public List<CourseGradeDto> Courses { get; set; } = new();
    }

    public class CourseGradeDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public decimal TotalScore { get; set; }
        public LetterGrade LetterGrade { get; set; }
        public decimal GradePoint { get; set; }
        public bool IsPassed { get; set; }
    }

}
