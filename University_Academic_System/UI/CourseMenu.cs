using Microsoft.Identity.Client.NativeInterop;
using System;
using University_Academic_System.Entities;
using University_Academic_System.Enums;
using University_Academic_System.Services;

namespace University_Academic_System.UI
{
    public class CourseMenu
    {
        private readonly ICourseService _courseService;

        public CourseMenu(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Course Management Menu ---");
                Console.WriteLine("1. Add New Course");
                Console.WriteLine("2. View All Courses");
                Console.WriteLine("3. Add Prerequisite");
                Console.WriteLine("4. View Course Prerequisites");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    var course = new Course();
                    Console.Write("Enter Course ID: ");
                    course.Id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter Course Code: ");
                    course.Code = Console.ReadLine() ?? "";
                    Console.Write("Enter Course Title: ");
                    course.Title = Console.ReadLine() ?? "";
                    Console.Write("Enter Credit Hours (1-6): ");
                    course.CreditHours = int.Parse(Console.ReadLine() ?? "3");
                    course.Level = (int)AcademicLevel.FirstYear;

                    _courseService.AddCourse(course);
                    Console.WriteLine("Course added successfully!");
                }
                else if (choice == "2")
                {
                    var courses = _courseService.GetAllCourses();
                    Console.WriteLine("\n--- Courses List ---");
                    foreach (var c in courses)
                    {
                        Console.WriteLine($"ID: {c.Id}, Code: {c.Code}, Title: {c.Title}, Credits: {c.CreditHours}");
                    }
                }
                else if (choice == "3")
                {
                    Console.Write("Enter Course ID: ");
                    int courseId = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter Prerequisite Course ID: ");
                    int prereqId = int.Parse(Console.ReadLine() ?? "0");

                    _courseService.AddPrerequisite(courseId, prereqId);
                    Console.WriteLine("Prerequisite added successfully!");
                }
                else if (choice == "4")
                {
                    Console.Write("Enter Course ID to view prerequisites: ");
                    int courseId = int.Parse(Console.ReadLine() ?? "0");
                    var prereqs = _courseService.GetPrerequisites(courseId);

                    Console.WriteLine("\n--- Prerequisites ---");
                    foreach (var p in prereqs)
                    {
                        Console.WriteLine($"- {p.Code}: {p.Title}");
                    }
                }
                else if (choice == "5")
                {
                    break;
                }
            }
        }
    }
}