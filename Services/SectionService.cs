using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;
namespace University_Academic.Services
{
    public class SectionService
    {
        private readonly UniversityDbContext _context;

        public SectionService(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult CreateSection(int courseId, int instructorId, int semesterId, int classroomId, int capacity)
        {
            var course = _context.Courses.FirstOrDefault(e => e.CourseId == courseId);
            if (course == null)
                return Failure("Can't Found Course");
            var instructor = _context.Instructors.FirstOrDefault(e => e.InstructorId == instructorId);
            if (instructor == null)
                return Failure("Can't Found Instructor");
            if (instructor.Status != Enums.InstructorStatus.Active)
                return Failure("Instructor Not Active");
            if(instructor.DepartmentId != course.DepartmentId)
            {
                return Failure("Cannot assign instructor: The instructor belongs to a different academic department than the course .");
            }

            var semester = _context.Semesters.FirstOrDefault(e => e.SemesterId == semesterId);
            if (semester == null)
                return Failure("Can't Found Semester");
            if (semester.Status != SemesterStatus.Active && semester.Status != SemesterStatus.Upcoming)
                return Failure("A section can only be created in an Active or Upcoming semester.");

            var classroom = _context.Classrooms.FirstOrDefault(e => e.ClassroomId == classroomId);
            if (classroom == null)
                return Failure("Can't Found Classroom");

            if (capacity > classroom.Capacity)
                return Failure("Section Capacity is greater than Classroom Capacity");

            int currentLoad = _context.CourseSections.Count(s => s.InstructorId == instructorId && s.SemesterId == semesterId);
            if (currentLoad >= 5)
                return Failure($"Exceeded maximum limit! The instructor is assigned {currentLoad} section(s) in this semester, and the maximum allowed is 5 sections .");
            
            int lastSectionNumber = _context.CourseSections
             .Where(s => s.CourseId == courseId && s.SemesterId == semesterId)
             .Select(s => (int?)s.SectionNumber)
             .Max() ?? 0;

            int newSectionNumber = lastSectionNumber + 1;
            

            var section = new CourseSection
            {
                CourseId = courseId,
                InstructorId = instructorId,
                SemesterId = semesterId,
                ClassroomId = classroomId,
                Capacity = capacity,
                SectionNumber = newSectionNumber
            };
            _context.CourseSections.Add(section);
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult AddScheduleSlot(int sectionId, StudyDayOfWeek day, TimeSpan startTime, TimeSpan endTime)
        {
            var section = _context.CourseSections.FirstOrDefault(e => e.SectionId == sectionId);
            if (section == null)
            {
                return Failure("Can't found Section");
            }
            TimeSpan duration = endTime - startTime;
            if (startTime >= endTime || duration.TotalMinutes < 45)
            {
                return Failure($"Duration is only {duration.TotalMinutes} mins. Minimum required is 45 minutes!");
            }

            var conflict = CheckScheduleConflict(section.SemesterId, section.ClassroomId, section.InstructorId, day, startTime, endTime, sectionId);
            if (!conflict.IsSuccess)
                return conflict;

            var sectionSchedule = new SectionSchedule
            {
                SectionId = sectionId,
                DayOfWeek = day,
                StartTime = startTime,
                EndTime = endTime
            };
            _context.SectionSchedules.Add(sectionSchedule);
            _context.SaveChanges();

            return Success();
        }
        ServiceResult CheckScheduleConflict(int semesterId, int classroomId, int instructorId,
            StudyDayOfWeek day, TimeSpan newStart, TimeSpan newEnd,
            int currentSectionId)
        {
            //============Room check
            var roomIsBooked = _context.SectionSchedules
                .Where(e => e.Section.SemesterId == semesterId
                            && e.Section.ClassroomId == classroomId
                            && e.DayOfWeek == day
                            && e.SectionId != currentSectionId
                            && newStart < e.EndTime && newEnd > e.StartTime)
                 .Select(e => new
                 {
                     e.Section.Course.Code,
                 }).FirstOrDefault();
            if (roomIsBooked != null)
            {
                return Failure($"Classroom conflict! The room is booked for course {roomIsBooked.Code}");
            }

            // Instructor check
            var instructorIsFree = _context.SectionSchedules
                .Where(e => e.Section.SemesterId == semesterId
                          && e.Section.InstructorId == instructorId
                          && e.DayOfWeek == day
                          && e.SectionId != currentSectionId
                          && newStart < e.EndTime && newEnd > e.StartTime)
                .Select(e => new
                {
                    e.Section.Course.Code
                }).FirstOrDefault();
            if (instructorIsFree != null)
            {
                return Failure($"Instructor conflict! Instructor has Lec for course {instructorIsFree.Code}");
            }

            //=================
            var internalConflict = _context.SectionSchedules
                .Where(e => e.SectionId == currentSectionId 
                            && e.DayOfWeek == day
                            && newStart < e.EndTime 
                            && newEnd > e.StartTime)
                .FirstOrDefault();
            if (internalConflict != null)
                return Failure($"Internal conflict! The section already has a scheduled lecture on the same day from {internalConflict.StartTime:hh\\:mm} to {internalConflict.EndTime:hh\\:mm}.");



            return Success();
        }

        public ServiceResult DeleteScheduleSlot(int scheduleId)
        {
            var schedule = _context.SectionSchedules.FirstOrDefault(e => e.ScheduleId == scheduleId);
            if (schedule == null)
            {
                return Failure("Can't found Schedule ");
            }
            _context.SectionSchedules.Remove(schedule);
            _context.SaveChanges();

            return Success();
        }
        public ServiceResult UpdateCapacity(int sectionId, int newCapacity)
        {
            var schedule = _context.CourseSections
                .Include(e=>e.Classroom)
                .FirstOrDefault(e => e.SectionId == sectionId);
            if (schedule == null)
            {
                return Failure("Can't found Section ");
            }
            if(newCapacity <= 0 )
            {
                return Failure("Capacity must be > 0");
            }
            if(newCapacity> schedule.Classroom.Capacity)
            {
                return Failure($"Can't Update Capacity ,the max Capacity of classroom is {schedule.Classroom.Capacity} ");
            }
            int enrolledCount = _context.Enrollments.Count(e => e.SectionId == schedule.SectionId); 
            if (newCapacity < enrolledCount)
                return Failure($"Cannot reduce capacity to {newCapacity}; the number of currently enrolled students is {enrolledCount}.");

            schedule.Capacity = newCapacity;
            _context.SaveChanges();
            return Success();
        }
        public List<CourseSection> GetSectionsBySemester(int semesterId)
        {
            return _context.CourseSections.Where(e => e.SemesterId == semesterId)
                .Include(e=>e.Instructor)
                .Include(e=>e.SectionSchedules)
                .ToList();
        }
        public List<CourseSection> GetSectionsByCourse(int courseId, int semesterId)
        {
            return _context.CourseSections.Where(e => e.SemesterId == semesterId && e.CourseId == courseId).ToList();
        }
        public CourseSection ? GetSectionDetails(int sectionId)
        {
            return _context.CourseSections
                .Include(e=>e.SectionSchedules)
                .FirstOrDefault(e=>e.SectionId == sectionId);
        }

        public List<Course> GetAvailableCourses()
        {
            return _context.Courses
                .Select(c => new Course { CourseId = c.CourseId, Name = c.Name, Code = c.Code })
                .ToList();
        }
        public List<CourseSection> GetAvailableSections()
        {
            return _context.CourseSections
                .Select(c => new CourseSection { SectionId = c.SectionId, SectionNumber = c.SectionNumber  })
                .ToList();
        }
        public List<Semester> GetAvailableSemester()
        {
            return _context.Semesters
                .Select(c => new Semester { SemesterId  = c.SemesterId, Name = c.Name})
                .ToList();
        }
        public List<SectionSchedule> GetAvailableSchedule()
        {
            return _context.SectionSchedules
                .Select(c => new SectionSchedule { ScheduleId  = c.ScheduleId, DayOfWeek = c.DayOfWeek})
                .ToList();
        }
        public List<Instructor> GetAvailableInstructor()
        {
            return _context.Instructors
                .Where(e=>e.Status == InstructorStatus.Active)
                .Select(c => new Instructor { InstructorId = c.InstructorId, FullName = c.FullName})
                .ToList();
        }
        public List<Classroom> GetAvailableClassroom()
        {
            return _context.Classrooms
                .Select(c => new Classroom { ClassroomId = c.ClassroomId, Building = c.Building })
                .ToList();
        }

    }
}