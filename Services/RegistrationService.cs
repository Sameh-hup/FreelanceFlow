using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;

namespace University_Academic.Services
{
    public class StudentScheduleSlotDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int SectionNumber { get; set; }
        public string Day { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Classroom { get; set; } = string.Empty;

    }

    public class RegistrationService
    {
        private readonly UniversityDbContext _context;
        private readonly GpaService _gpaService; 
        public RegistrationService(UniversityDbContext context , GpaService gpaService)
        {
            _context = context;
            _gpaService = gpaService;
        }

        public ServiceResult RegisterStudent(int studentId, int sectionId)
        {
            //get Student
            var student = _context.Students
                .Include(e=>e.Enrollments)
                .FirstOrDefault(e => e.StudentId == studentId);
            if (student == null)
            {
                return Failure("Student Id Not Exist ");
            }
            //get section
            var section = _context.CourseSections
                .Include(e => e.Course)
                    .ThenInclude(e => e.Prerequisites)
                        .ThenInclude(e => e.PrerequisiteCourse)
                .Include(s => s.Semester)
                .Include(s => s.Classroom)
                .Include(s => s.SectionSchedules)
                .Include(s => s.Enrollments)
                .FirstOrDefault(e => e.SectionId == sectionId);
            if (section == null)
            {
                return Failure("CourseSections Id Not Exist ");
            }

            if (!section.SectionSchedules.Any())
            {
                return Failure("Cannot register for this section because no class schedules have been recorded for it yet.");
            }
            if (section.Semester.Status != SemesterStatus.Active)
            { return Failure($"Registration cannot be completed because semester Not active "); }
            if (student.Status != StudentStatus.Active)
            {
                return Failure($"Registration cannot be completed because Student Not active ");
            }
            //Is Student Already Registration or Completed Section
            var existingEnrollmentInSection = _context.Enrollments
            .FirstOrDefault(e => e.StudentId == studentId && e.SectionId == sectionId);
            if (existingEnrollmentInSection != null)
            {
                if (existingEnrollmentInSection.Status == EnrollmentStatus.Registered)
                {
                    return Failure("Student Is Already Registration in this section");
                }
                if (existingEnrollmentInSection.Status == EnrollmentStatus.Completed)
                {
                    return Failure("Student is competed this section");
                }
            }
            //Registered In Course
            var isAlreadyRegisteredInSameCourse = _context.Enrollments
                .Any(e => e.StudentId == studentId
              && e.Section.CourseId == section.CourseId
              && e.Section.SemesterId == section.SemesterId
              && e.SectionId != sectionId
              && e.Status == EnrollmentStatus.Registered);

            if (isAlreadyRegisteredInSameCourse)
            {
                return Failure($"The student is already registered in another section for course ({section.Course.Name}) in the current semester.");

            }
            var hasPassedBefore = _context.Enrollments
            .Include(e => e.Grade)
            .Any(e => e.StudentId == studentId
                   && e.Section.CourseId == section.CourseId
                   && e.Status == EnrollmentStatus.Completed
                   && e.Grade != null
                   && e.Grade.Total >= 60.00m);
            if (hasPassedBefore)
            {
                return Failure($"the student has already passed course ({section.Course.Code} - {section.Course.Name}) successfully ");
            }

            var missingPrerequisites = new List<string>();
            foreach (var prereq in section.Course.Prerequisites)
            {
                bool prereqPassed = _context.Enrollments
                    .Include(e => e.Grade)
                    .Any(e => e.StudentId == studentId
                           && e.Section.CourseId == prereq.PrerequisiteCourseId
                           && e.Status == EnrollmentStatus.Completed
                           && e.Grade != null
                           && e.Grade.Total >= 60.00m);
                if (!prereqPassed)
                {
                    missingPrerequisites.Add($"{prereq.PrerequisiteCourse.Code} - {prereq.PrerequisiteCourse.Name}");
                }
            }
            if (missingPrerequisites.Any())
            {
                return Failure( $"Cannot register for the course because not all prerequisites have been passed successfully" );
            }
            //AcademicLevel
            if (section.Course.Level > student.AcademicLevel)
            {
                return Failure(
                 $"The course level ({section.Course.Level}) is higher than the student's current academic level ({student.AcademicLevel}).");
            }
            //Student week Schedule with Registration
            var currentStudentSchedules = _context.SectionSchedules
                .Include(ss => ss.Section).ThenInclude(sec => sec.Course)
                .Where(ss => ss.Section.SemesterId == section.SemesterId
                          && ss.Section.Enrollments.Any(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Registered))
                .ToList();
            foreach (var newSlot in section.SectionSchedules)
            {
                var conflictSlot = currentStudentSchedules
                    .FirstOrDefault(existing => existing.DayOfWeek == newSlot.DayOfWeek
                                             && newSlot.StartTime < existing.EndTime
                                             && newSlot.EndTime > existing.StartTime);
                if (conflictSlot != null)
                {
                    return Failure("Schedule conflict! The selected time slot overlaps with another registered course.");
                }
            }
            //section Capacity
            int currentOccupancy = _context.Enrollments
                .Count(e => e.SectionId == sectionId
                         && (e.Status == EnrollmentStatus.Registered || e.Status == EnrollmentStatus.Completed));
            if (currentOccupancy >= section.Capacity)
            {
                
                return Failure("Section is full! Maximum class capacity has been reached.");
            }
          
            decimal cgpa = _gpaService.CalculateCgpa(studentId);
            int maxAllowedHours = (cgpa < 2.00m && cgpa > 0.00m) ? 12 : 18;
            int currentRegisteredHours = _context.Enrollments
                .Where(e => e.StudentId == studentId
                         && e.Section.SemesterId == section.SemesterId
                         && e.Status == EnrollmentStatus.Registered
                         && e.SectionId != sectionId)
                .Sum(e => (int)e.Section.Course.CreditHours);

            if (currentRegisteredHours + section.Course.CreditHours > maxAllowedHours)
            {
                string standingText = (maxAllowedHours == 12) ? "Academic Probation" : "Regular Student";
                return Failure("Credit hours limit exceeded! Adding this course exceeds your maximum allowed hours.");
            }

            if (existingEnrollmentInSection != null && existingEnrollmentInSection.Status == EnrollmentStatus.Dropped)
            {
                existingEnrollmentInSection.Status = EnrollmentStatus.Registered;
                existingEnrollmentInSection.RegistrationDate = DateTime.Now;
                _context.SaveChanges();
                return Success();
            }
            // إنشاء سجل تسجيل جديد
            var newEnrollment = new Enrollment
            {
                StudentId = studentId,
                SectionId = sectionId,
                RegistrationDate = DateTime.Now,
                Status = EnrollmentStatus.Registered
            };
            _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();
            return Success();
        }

        public ServiceResult DropCourse(int studentId, int sectionId)
        {
            var enrollment = _context.Enrollments
                .Include(e => e.Section).ThenInclude(s => s.Semester)
                .Include(e => e.Grade)
                .FirstOrDefault(e => e.StudentId == studentId && e.SectionId == sectionId);
            if (enrollment == null || enrollment.Status != EnrollmentStatus.Registered)
            {
                return Failure("Student is not currently enrolled in this section.");
            }
           
            if (enrollment.Section.Semester.Status != SemesterStatus.Active)
            {
                return Failure("Courses can only be dropped during an active semester.");
            }
           
            if (enrollment.Grade != null)
            {
                return Failure("Cannot drop the course after grades have been posted.");
            }
            enrollment.Status = EnrollmentStatus.Dropped;
            _context.SaveChanges();
            return Success();
        }
        
        public List<CourseSection> GetAvailableSectionsForStudent(int studentId, int semesterId)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null || student.Status != StudentStatus.Active)
                return new List<CourseSection>();
           
            var passedCourseIds = _context.Enrollments
                .Include(e => e.Grade)
                .Where(e => e.StudentId == studentId
                         && e.Status == EnrollmentStatus.Completed
                         && e.Grade != null
                         && e.Grade.Total >= 60.00m)
                .Select(e => e.Section.CourseId)
                .Distinct()
                .ToList();
            
            var allSections = _context.CourseSections
                .Include(s => s.Course).ThenInclude(c => c.Prerequisites)
                .Include(s => s.Instructor)
                .Include(s => s.Classroom)
                .Include(s => s.SectionSchedules)
                .Include(s => s.Enrollments)
                .Where(s => s.SemesterId == semesterId && s.SectionSchedules.Any())
                .ToList();
            var availableSections = new List<CourseSection>();
            foreach (var sec in allSections)
            {
                
                if (passedCourseIds.Contains(sec.CourseId)) continue;
                
                if (sec.Course.Level > student.AcademicLevel) continue;
                
                int enrolledCount = sec.Enrollments.Count(e => e.Status == EnrollmentStatus.Registered || e.Status == EnrollmentStatus.Completed);
                if (enrolledCount >= sec.Capacity) continue;
                
                bool allPrereqsMet = sec.Course.Prerequisites.All(p => passedCourseIds.Contains(p.PrerequisiteCourseId));
                if (!allPrereqsMet) continue;
                availableSections.Add(sec);
            }
            return availableSections;
        }
        
        public List<Enrollment> GetStudentCurrentEnrollments(int studentId, int semesterId)
        {
            return _context.Enrollments
                .Include(e => e.Section).ThenInclude(s => s.Course)
                .Include(e => e.Section).ThenInclude(s => s.Instructor)
                .Include(e => e.Section).ThenInclude(s => s.Classroom)
                .Include(e => e.Section).ThenInclude(s => s.SectionSchedules)
                .Where(e => e.StudentId == studentId
                         && e.Section.SemesterId == semesterId
                         && e.Status == EnrollmentStatus.Registered)
                .ToList();
        }
        
        public int GetCurrentSemesterCreditHours(int studentId, int semesterId)
        {
            return _context.Enrollments
                .Where(e => e.StudentId == studentId
                         && e.Section.SemesterId == semesterId
                         && e.Status == EnrollmentStatus.Registered)
                .Sum(e => (int)e.Section.Course.CreditHours);
        }
        
        public List<StudentScheduleSlotDto> GetStudentWeeklySchedule(int studentId, int semesterId)
        {
            return _context.SectionSchedules
                .Include(ss => ss.Section).ThenInclude(s => s.Course)
                .Include(ss => ss.Section).ThenInclude(s => s.Classroom)
                .Where(ss => ss.Section.SemesterId == semesterId
                          && ss.Section.Enrollments.Any(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Registered))
                .OrderBy(ss => ss.DayOfWeek)
                .ThenBy(ss => ss.StartTime)
                .Select(ss => new StudentScheduleSlotDto
                {
                    CourseCode = ss.Section.Course.Code,
                    CourseName = ss.Section.Course.Name,
                    SectionNumber = ss.Section.SectionNumber,
                    Day = ss.DayOfWeek.ToString(),
                    StartTime = ss.StartTime,
                    EndTime = ss.EndTime,
                    Classroom = $"{ss.Section.Classroom.Building} - {ss.Section.Classroom.RoomNumber}"
                })
                .ToList();
        }
        public List<Student> GetStudents()
        {
            return _context.Students
                .Include(e => e.Enrollments)
                .Include(e => e.Department)
                .ToList();
        }
        public List<Semester> GetAvailableSemester()
        {
            return _context.Semesters
                .Select(c => new Semester { SemesterId = c.SemesterId, Name = c.Name })
                .ToList();
        }
    }
}

