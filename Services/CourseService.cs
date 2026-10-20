using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;

namespace University_Academic.Services
{
    public class CourseService
    {
        private readonly UniversityDbContext _context;
        public CourseService(UniversityDbContext context)
        {
            _context = context;
        }

        public ServiceResult AddCourse(string name, string code, int creditHours,AcademicLevel level, int departmentId)
        {
            bool isCodeExists = _context.Courses.Any(c => c.Code == code);
            if (isCodeExists)
            {
                return Failure("Course code already exists.");
            }
            bool isDepartmentExists = _context.Departments.Any(d => d.DepartmentId == departmentId);
            if (!isDepartmentExists)
            {
                return Failure("Department does not exist.");
            }
            
            var course = new Course
            {
                Name = name,
                Code = code,
                CreditHours = creditHours,
                Level = level,
                DepartmentId = departmentId
            };
            _context.Courses.Add(course);
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult UpdateCourse(int courseId, string name, string code, int creditHours, AcademicLevel level, int departmentId)
        {
            var course =_context.Courses.Find(courseId);
            if (course == null) return Failure("Course not found.");
            course.Name = name;
            course.Code = code;
            course.CreditHours = creditHours;
            course.Level = level;
            course.DepartmentId = departmentId;
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult DeleteCourse(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return Failure("Course not found.");
            bool hasSections = _context.CourseSections.Any(s => s.CourseId == courseId);
            if (hasSections) return Failure("Cannot delete course because it has active/open sections.");

            bool isPrereqForOthers = _context.CoursePrerequisites.Any(cp => cp.PrerequisiteCourseId == courseId);
            if (isPrereqForOthers) return Failure("Cannot delete course because it is a prerequisite for other courses.");
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult AddPrerequisite(int courseId, int prerequisiteCourseId)
        {
            if(courseId == prerequisiteCourseId) return Failure("Course cannot be a prerequisite of itself.");

            var course = _context.Courses
                .FirstOrDefault(e=>e.CourseId == courseId);
            var prereqCourse = _context.Courses
                .FirstOrDefault(e => e.CourseId == prerequisiteCourseId);

            if(course == null || prereqCourse == null) return Failure("Course or prerequisite course not found.");
            if (course.Level < prereqCourse.Level)  return Failure("Prerequisite course level cannot be higher than the course level.");
            bool Exist = _context.CoursePrerequisites.Any(cp => cp.CourseId == courseId && cp.PrerequisiteCourseId == prerequisiteCourseId);

            if (Exist)
            {
                return Failure($"this PrerequisiteCourse{prereqCourse.Code} Is already Add to this course {course.Code}");
            }
            if (HasCycle(courseId,prerequisiteCourseId))
            {
                return Failure("Error: Cannot add this prerequisite because it creates a circular dependency loop.");
            }
            var prerequisiteCourse = new CoursePrerequisite
            {
                Course =course,
                PrerequisiteCourse = prereqCourse
            };
            course.Prerequisites.Add(prerequisiteCourse);
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult RemovePrerequisite(int courseId, int prerequisiteCourseId)
        {
            if( courseId == prerequisiteCourseId )  return Failure("Course cannot be a prerequisite of itself.") ;

            
            var prerequisiteCourse = _context.CoursePrerequisites
                .FirstOrDefault(e=> e.CourseId == courseId 
                && e.PrerequisiteCourseId == prerequisiteCourseId);
            if (prerequisiteCourse == null) return Failure("Prerequisite course not found.");

            _context.CoursePrerequisites.Remove(prerequisiteCourse);
            _context.SaveChanges();
            return Success();
        }
        public List<Course> GetAllCourses()
        {
            return _context.Courses.Include(c => c.Department).Include(c => c.Prerequisites).ToList();
        }
        //public ServiceResult GetCourseAndPrerequisite(int courseId)
        //{
        //    var course = _context.Courses
        //        .Where(e => e.CourseId == courseId)
        //        .Select(e => new
        //        {
        //            e.CourseId,
        //            e.Name,
        //            e.Code,
                   

                    
                    
        //        });
        //    return Success();
        //}
        public List<Course> GetAvailableCourses()
        {
            return _context.Courses
                .Select(c => new Course { CourseId = c.CourseId, Name = c.Name, Code = c.Code })
                .ToList();
        }

        //=======
        public bool HasCycle(int courseId, int newPrereqId)
        {
            if (courseId == newPrereqId) return true;

            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(newPrereqId);

            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                if (currentId == courseId)
                    return true; 

                if (visited.Add(currentId))
                {
                    var parentPrereqs = _context.CoursePrerequisites
                        .Where(cp => cp.CourseId == currentId)
                        .Select(cp => cp.PrerequisiteCourseId)
                        .ToList();

                    foreach (var prereq in parentPrereqs)
                    {
                        if (!visited.Contains(prereq))
                            stack.Push(prereq);
                    }
                }
            }
            return false;
        }
    }
}
