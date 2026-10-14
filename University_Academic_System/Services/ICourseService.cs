using Microsoft.Identity.Client.NativeInterop;
using System.Collections.Generic;
using University_Academic_System.Entities;

namespace University_Academic_System.Services
{
    public interface ICourseService
    {
        void AddCourse(Course course);
        List<Course> GetAllCourses();
        Course GetCourseById(int id);
        void AddPrerequisite(int courseId, int prerequisiteId);
        void RemovePrerequisite(int courseId, int prerequisiteId);
        List<Course> GetPrerequisites(int courseId);
    }
}