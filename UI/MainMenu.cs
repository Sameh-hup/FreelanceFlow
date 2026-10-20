using System;
using University_Academic.Services;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ConsoleHelper;
using University_Academic.Entities;
namespace University_Academic.UI
{
    public class MainMenu
    {
        private readonly CollegeService _collegeService;
        private readonly DepartmentService _departmentService;
        private readonly ClassroomService _classroomService;
        private readonly SemesterService _semesterService;
        private readonly CourseService _courseService;
        private readonly InstructorService _instructorService;
        private readonly SectionService _sectionService;
        private readonly StudentService _studentService;
        private readonly RegistrationService _registrationService;
        private readonly GradeServices _gradeServices;
        private readonly GpaService _gpaService;
        

        public MainMenu(CollegeService collegeService, DepartmentService departmentService, ClassroomService classroomService 
            ,SemesterService semesterService, CourseService courseService, InstructorService instructorService 
            , SectionService sectionService ,StudentService studentService ,RegistrationService registrationService, GradeServices gradeServices, GpaService gpaService)
        {
            _collegeService = collegeService;
            _departmentService = departmentService;
            _classroomService = classroomService;
            _semesterService = semesterService;
            _courseService = courseService;
            _instructorService= instructorService;
            _sectionService = sectionService;
            _studentService= studentService;
            _registrationService= registrationService;
            _gradeServices =gradeServices;
            _gpaService = gpaService;
        }

        public void Show()
        {
            bool isRunning = true;

            do
            {
                Console.Clear();
                Print();

                int choice = ConsoleHelper.ReadInt("Choose : ");
                if (choice == -1)
                {
                    continue;
                }
                switch (choice)
                {
                    case 1:
                        var collegeMenu = new CollegeMenu(_collegeService);
                        collegeMenu.Show();
                        break;
                    case 2:
                        var departmentMenu = new DepartmentMenu(_departmentService);
                        departmentMenu.Show();
                        break;
                    case 3:     
                        var studentMenu =new StudentMenu(_studentService);
                        studentMenu.Show();
                        break;
                    case 4:
                        var instrctor =new InstructorMenu(_instructorService); 
                        instrctor.Show();
                        break;
                    case 5:
                        var courseMenu = new CourseMenu(_courseService);
                        courseMenu.Show();
                        break;
                    case 6:
                        var semester = new SemesterMenu(_semesterService);
                        semester.Show();
                        break;
                    case 7:
                        var classroom = new ClassroomMenu(_classroomService);
                        classroom.Show();
                        break;
                    case 8:
                        var section = new SectionMenu(_sectionService);
                        section.Show();
                        break;
                    case 9:
                        var registration = new RegistrationMenu(_registrationService);
                        registration.Show(); break;
                    case 10:
                        var grade = new GradeMenu(_gradeServices,_gpaService);
                        grade.Show();
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                        ShowErrorMessage("\nInvalid choice! Please try again.");
                        break;
                }
            } while (isRunning);
        }

        #region UI Formatting Methods
        private void Print()
        {
            var mainMenuOptions = new List<string>
                    {
                        "College Management",       // 1
                        "Department Management",    // 2
                        "Student Management",       // 3
                        "Instructor Management",    // 4
                        "Course Management",        // 5
                        "Semester Management",      // 6
                        "Classroom Management",     // 7
                        "Course Section Management",// 8
                        "Course Registration",      // 9
                        "Grade Management",         // 10
                    };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", mainMenuOptions);
            
        }
        #endregion
    }
}