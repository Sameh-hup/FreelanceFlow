using University_Academic.Enums;
using University_Academic.Services;
using static University_Academic.Helper_Function.ConsoleHelper;
namespace University_Academic.UI
{
    public class CourseMenu
    {
        private readonly CourseService _courseService;
        public CourseMenu(CourseService courseService)
        {
            _courseService = courseService;
        }

        public void Show()
        {
            bool IsRunning = true;

            do
            {
                Console.Clear();
                Print();

                Console.ResetColor();
                int input = ReadInt("\n Choose : ");
                if (input == -1)
                {
                    continue;
                }
                switch (input)
                {
                    case 1:
                        addCourse();
                        break;
                    case 2:
                        updateCourse(); 
                        break;
                    case 3:
                        deleteCourse();
                        break;
                    case 4:
                        addPrerequisite();
                        break;
                    case 5:
                        removePrerequisite();
                        break;
                    case 6:
                        viewAllCourses();
                        break;
                    case 0:
                        IsRunning = false;
                        break;
                    default:
                        ShowErrorMessage(" Invalid option! Please select between 1 and 6.");
                        break;
                }

            } while (IsRunning);
        }

        private void Print()
        {
            Console.Clear();
            var CourseMenuOptions = new List<string>
                    {

                        "Add Course",
                        "Update Course",
                        "Delete Course",
                        "Add Prerequisite",
                        "Remove Prerequisite",
                        "View All Courses"
                    };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", CourseMenuOptions);
        }

        void addCourse()
        {
            Console.Clear();
            PrintTitle("Add Course");

            string name = chekstring("Enter Course Name: ");
            string code = chekstring("Enter Course Code: ");
            int creditHours = chekInt("Enter Credit Hours ( 1 - 6 ) : ");
            int levelNum = chekInt("Enter Academic Level Number (FirstYear = 1,\r\nSecondYear = 2,\r\nThirdYear = 3,\r\nFourthYear = 4): ");
            AcademicLevel level = (AcademicLevel)levelNum;
            int departmentId = chekInt("Enter Department ID: ");

            var course = _courseService.AddCourse(name, code, creditHours, level, departmentId);
            if(course.IsSuccess)
            {
                ShowSuccessMessage("Course added successfully.");
            }
            else
            {
                ShowErrorMessage($"can't add course: {course.ErrorMessage}");
            }


        }
        void updateCourse()
        {
            Console.Clear();
            PrintTitle("Update Course");

            int courseId = chekInt("Enter Course ID to update: ");
            string name = chekstring("Enter new Course Name: ");
            string code = chekstring("Enter new Course Code: ");
            int creditHours = chekInt("Enter new Credit Hours (1 - 6 ) : ");
            int levelNum = chekInt("Enter new Academic Level Number (FirstYear = 1,\r\nSecondYear = 2,\r\nThirdYear = 3,\r\nFourthYear = 4): ");
            AcademicLevel level = (AcademicLevel)levelNum;
            int departmentId = chekInt("Enter new Department ID: ");

            var result = _courseService.UpdateCourse(courseId, name, code, creditHours, level, departmentId);
            if(result.IsSuccess)
            {
                ShowSuccessMessage("Course updated successfully.");
            }
            else
            {
                ShowErrorMessage($"can't update course: {result.ErrorMessage}");
            }
        }
        void deleteCourse()
        {
            Console.Clear();
            PrintTitle("Delete Course");
            int courseId = chekInt("Enter Course ID to delete: ");
            var result = _courseService.DeleteCourse(courseId);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("Course deleted successfully.");
            }
            else
            {
                ShowErrorMessage($"can't delete course: {result.ErrorMessage}");
            }
        }
        void addPrerequisite()
        {
            Console.Clear();
            PrintTitle("Add Prerequisite");
            int courseId = chekInt("Enter Course ID: ");
            int prerequisiteId = chekInt("Enter Prerequisite Course ID: ");
            var result = _courseService.AddPrerequisite(courseId, prerequisiteId);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("Prerequisite added successfully.");
            }
            else
            {
                ShowErrorMessage($"can't add prerequisite: {result.ErrorMessage}");
            }
        }
        void removePrerequisite()
        {
            Console.Clear();
            PrintTitle("Remove Prerequisite");
            int courseId = chekInt("Enter Course ID: ");
            int prerequisiteId = chekInt("Enter Prerequisite Course ID: ");
            var result = _courseService.RemovePrerequisite(courseId, prerequisiteId);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("Prerequisite removed successfully.");
            }
            else
            {
                ShowErrorMessage($"can't remove prerequisite: {result.ErrorMessage}");
            }
        }

        void viewAllCourses()
        {
            Console.Clear();
            PrintTitle("All Courses");


            var Courses = _courseService.GetAllCourses();
            if (Courses == null || !Courses.Any())
            {
                ShowWarningMessage("No Courses found.");
                return;
            }

            string[] header = { "ID", "Code", "Name", "CreditHours","Level","DepartmentId" ,"PrerequisiteID","PrerequisiteCode"};
            var rows = Courses.Select(e => new string[]
            {
                $"[white]{e.CourseId}[/]",
                $"[green]{e.Code}[/]",
                $"[cyan]{e.Name}[/]",
                $"[white]{e.CreditHours}[/]",
                $"[green]{e.Level}[/]",
                $"[white]{e.DepartmentId}[/]",
                $"[cyan]{string.Join(",",e.Prerequisites.Select(e=>e.PrerequisiteCourseId))}[/]",
                $"[red]{string.Join(",",e.Prerequisites.Select(e=>e.PrerequisiteCourse.Code))}[/]"

            });
            PrintTable(header, rows,rowSeparators:true);
        }
    }
}
