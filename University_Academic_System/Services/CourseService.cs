using Microsoft.Identity.Client.NativeInterop;
using System.Collections.Generic;
using System.Linq;
using University_Academic_System.Entities;

namespace University_Academic_System.Services
{
    public class CourseService : ICourseService
    {
        private static List<Course> courses = new List<Course>();

        public void AddCourse(Course course)
        {
            courses.Add(course);
        }

        public List<Course> GetAllCourses()
        {
            return courses;
        }

        public Course GetCourseById(int id)
        {
            return courses.FirstOrDefault(c => c.Id == id);
        }

        public void AddPrerequisite(int courseId, int prerequisiteId)
        {
        }

        public void RemovePrerequisite(int courseId, int prerequisiteId)
        {
        }

        public List<Course> GetPrerequisites(int courseId)
        {
            return new List<Course>();
        }
    }
}