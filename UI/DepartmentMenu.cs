
using University_Academic.Entities;
using University_Academic.Helper_Function;
using University_Academic.Services;
using static System.Collections.Specialized.BitVector32;
using static University_Academic.Helper_Function.ConsoleHelper;
namespace University_Academic.UI
{
    public class DepartmentMenu
    {
        private readonly DepartmentService _departmentService;

        public DepartmentMenu(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public void Show() 
        {
            bool isRunning = true;
            do
            {
                Console.Clear();
                Print();

                int choice = ConsoleHelper.chekInt("Choose : ");
                if (choice == -1)
                {
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddDepartmentUI();
                        break;
                    case 2:
                        ShowAllDepartmentsUI();
                        break;
                    case 3:
                        SearchDepartmentUI();
                        break;
                    case 4:
                        UpdateDepartmentUI();
                        break;
                    case 5:
                        DeleteDepartmentUI();
                        break;
                    case 6:
                        ViewDepartmentInstructor();
                        break;
                    case 7:
                        ViewDepartmentStudents();
                        break;
                    case 8:
                        ViewDepartmentCourses();
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                       ShowErrorMessage("Invalid choice! Please select between 1 and 8.");
                        break;
                }

            } while (isRunning);
        }

        #region UI Drawing & Formatting

        private void Print()
        {
            Console.Clear();
            var CollegeMenuOptions = new List<string>
                    {
                        "Add Department",
                        " View Department",
                        "Search Department",
                        "Update Department",
                        "Delete Department",
                        "View Instructors",
                        "View Students",
                        "View Courses"
                    };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", CollegeMenuOptions);
        }

        #endregion

        #region Operations UI

        private void AddDepartmentUI()
        {
            Console.Clear();
            PrintTitle("Add New Department");

            string name = chekstring("Enter Department Name: ");
            string code = chekstring("Enter Department Code (e.g., CS, IT): ");
            string description = chekstring("Enter Description (Optional): ");

            int collegeId ;
            var courses = _departmentService.GetAvailableCollege();
            var result = GetSelected(courses, c => c.CollegeId, c => c.Name, "College");
            if (result.IsSuccess) collegeId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            var done = _departmentService.AddDepartment(name, code, description, collegeId);
            if(done.IsSuccess)
            {
                ShowSuccessMessage("Department added successfully!");
            }
            else
            {
                ShowErrorMessage($"Failed to add department: {done.ErrorMessage}");
            }
        }

        private void ShowAllDepartmentsUI()
        {
            Console.Clear();
            PrintTitle("All Departments");
            

            var departments = _departmentService.GetAllDepartments();
            if (departments == null || !departments.Any())
            {
                ShowWarningMessage("No departments found.");
                return;
            }

            string[] header = { "ID", "Name", "Code", "College ID" };
            var rows = departments.Select(e => new string[]
            {
                $"[white]{e.DepartmentId}[/]",
                $"[cyan]{e.Name}[/]",
                $"[green]{e.Code}[/]",
                $"[white]{e.CollegeId}[/]"
                
            });
            PrintTable(header, rows);
            
        }

        void SearchDepartmentUI()
        {
            Console.Clear();
            PrintTitle("Search Department");
            

            string term = chekstring("Enter Name or Code to search: ");
            var results = _departmentService.SearchDepartments(term);

            if (results == null || !results.Any())
            {
                ShowWarningMessage("No matching departments found.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nFound {results.Count()} result(s):");
            Console.ResetColor();

            string[] header = { "ID", "Name", "Code", "College ID" };
            var rows = results.Select(e => new string[]
            {
                $"[white]{e.DepartmentId}[/]",
                $"[cyan]{e.Name}[/]",
                $"[green]{e.Code}[/]",
                $"[white]{e.CollegeId}[/]"

            });
            PrintTable(header, rows);

        }

        private void UpdateDepartmentUI()
        {
            Console.Clear( );
            PrintTitle("Update Department");
       

            int id = chekInt("Enter Department ID to update: ");
            string name = chekstring($"Enter New Name: ");
            string code = chekstring($"Enter New Code : ");
            string description = chekstring($"Enter New Description : ");
            int collegeId = chekInt($"Enter New College ID : ");

            var isUpdated = _departmentService.UpdateDepartment(id, name, code, description, collegeId);
            if (isUpdated.IsSuccess)
            {
                ShowSuccessMessage("Department updated successfully!");
            }
            else
            {
                ShowErrorMessage($"Failed to update department: {isUpdated.ErrorMessage}");
            }
        }

        private void DeleteDepartmentUI()
        {
            Console.Clear();
            PrintTitle("Delete Department");
            

            int id = chekInt("Enter Department ID to delete: ");

            var isDeleted = _departmentService.DeleteDepartment(id);
            if(isDeleted.IsSuccess)
            {
                ShowSuccessMessage("Department deleted successfully!");
            }
            else
            {
                ShowErrorMessage($"Failed to delete department: {isDeleted.ErrorMessage}");
            }
        }

        void ViewDepartmentCourses()
        {
            Console.Clear();
            PrintTitle("View Department Courses");
            var sections = _departmentService.GetAvailableDepartment();
            int departmentId;
            var result = GetSelected(sections, c => c.DepartmentId, c => c.Name.ToString(), "Department");
            if (result.IsSuccess) departmentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            var departmentResult = _departmentService.GetDepartmentById(departmentId);
            if (!departmentResult.IsSuccess || departmentResult.Data == null)
            {
                ShowErrorMessage(departmentResult.ErrorMessage);
                return;
            }
            var courses = _departmentService.GetDepartmentCourses(departmentId);
            if (courses == null || !courses.Any())
            {
                ShowWarningMessage($"No courses found for {departmentResult.Data.Name}");
                return;
            }
            string[] header = { "ID", "Name", "Code", "Credit Hours", "Level" };
            var rows = courses.Select(c=>new string[]
            {
                $"[white]{c.CourseId}[/]",
                $"[cyan]{c.Name}[/]",
                $"[green]{c.Code}[/]",
                $"[white]{c.CreditHours}[/]",
                $"[yellow]{c.Level}[/]"
            });
            PrintTable(header, rows);
        }
        void ViewDepartmentStudents()
        {
            Console.Clear();
            PrintTitle("View Department Students");
            var sections = _departmentService.GetAvailableDepartment();
            int departmentId;
            var result = GetSelected(sections, c => c.DepartmentId, c => c.Name.ToString(), "Department");
            if (result.IsSuccess) departmentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }
            
            var departmentResult = _departmentService.GetDepartmentById(departmentId);
            if (!departmentResult.IsSuccess || departmentResult.Data == null)
            {
                ShowErrorMessage(departmentResult.ErrorMessage);
                return;
            }
            var Students = _departmentService.GetDepartmentStudent(departmentId);
            if (Students == null || !Students.Any())
            {
                ShowWarningMessage($"No Student found for {departmentResult.Data.Name}");
                return;
            }
            string[] header = { "ID", "UniversityID", "Full Name", "Email", "Level" };
            var rows = Students.Select(c=>new string[]
            {
                $"[white]{c.StudentId}[/]",
                $"[cyan]{c.UniversityId}[/]",
                $"[green]{c.FullName}[/]",
                $"[white]{c.Email}[/]",
                $"[yellow]{c.AcademicLevel}[/]"
            });
            PrintTable(header, rows);
        }
        void ViewDepartmentInstructor()
        {
            Console.Clear();
            PrintTitle("View Department Instructor");
            var sections = _departmentService.GetAvailableDepartment();
            int departmentId;
            var result = GetSelected(sections, c => c.DepartmentId, c => c.Name.ToString(), "Department");
            if (result.IsSuccess) departmentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }
            
            var departmentResult = _departmentService.GetDepartmentById(departmentId);
            if (!departmentResult.IsSuccess || departmentResult.Data == null)
            {
                ShowErrorMessage(departmentResult.ErrorMessage);
                return;
            }
            var instructors = _departmentService.GetDepartmentInstructor(departmentId);
            if (instructors == null || !instructors.Any())
            {
                ShowWarningMessage($"No Instructor found for {departmentResult.Data.Name}");
                return;
            }
            string[] header = { "ID", "Full Name", "Email" };
            var rows = instructors.Select(c=>new string[]
            {
                $"[white]{c.InstructorId}[/]",
                $"[cyan]{c.FullName}[/]",
                $"[green]{c.Email}[/]",
               
            });
            PrintTable(header, rows);
        }
        #endregion
    }
}