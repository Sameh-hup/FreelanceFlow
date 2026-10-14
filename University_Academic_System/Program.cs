using System;
using University_Academic_System.Services;
using University_Academic_System.UI;

namespace University_Academic_System
{
    class Program
    {
        static void Main(string[] args)
        {
            // إنشاء كائنات الـ Services (Dependency Injection يدوي بسيط)
            ICourseService courseService = new CourseService();
            ISemesterService semesterService = new SemesterService();
            IClassroomService classroomService = new ClassroomService();

            // إنشاء كائنات الـ UI Menus
            CourseMenu courseMenu = new CourseMenu(courseService);
            SemesterMenu semesterMenu = new SemesterMenu(semesterService);
            ClassroomMenu classroomMenu = new ClassroomMenu(classroomService);

            while (true)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("    UNIVERSITY ACADEMIC SYSTEM - MAIN MENU");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Manage Courses & Prerequisites (Course Menu)");
                Console.WriteLine("2. Manage Semesters (Semester Menu)");
                Console.WriteLine("3. Manage Classrooms (Classroom Menu)");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option (1-4): ");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    courseMenu.ShowMenu();
                }
                else if (choice == "2")
                {
                    semesterMenu.ShowMenu();
                }
                else if (choice == "3")
                {
                    classroomMenu.ShowMenu();
                }
                else if (choice == "4")
                {
                    Console.WriteLine("Exiting system. Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice, please try again.");
                }
            }
        }
    }
}