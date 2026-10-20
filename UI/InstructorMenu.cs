using Spectre.Console;
using University_Academic.Enums;
using University_Academic.Services;
using static University_Academic.Helper_Function.ConsoleHelper;
namespace University_Academic.UI
{
    public class InstructorMenu
    {
        private readonly InstructorService _instructorService;

        public InstructorMenu(InstructorService instructorService)
        {
            _instructorService = instructorService;
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
                        AddInstructor();
                        break;
                    case 2:
                        ListAllInstructor();
                        break;
                    case 3:
                        UpdateInstructors();
                        break;
                    case 4:
                        ViewInstructor();
                        break;
                    case 5:
                        ResignInstructor();
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
            var InstructorMenuOptions = new List<string>
                    {

                        "Add Instructor",
                        "List All Instructor",
                        "Update Instructor",
                        "View Instructor Details",
                        "Resign Instructor"
                    };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", InstructorMenuOptions);
        }
        void AddInstructor()
        {
            Console.Clear();
            PrintTitle("Add Instructor ");
            string fullName = chekstring("Enter Full Name :");
            string email = chekstring("Enter Email : ");
            string phone = chekstring("Enter Phone : ");
            int Academicnum = chekInt("Enter Academic Title Num (TeachingAssistant = 1\r\n    Lecturer = 2\r\n    AssistantProfessor = 3\r\n    AssociateProfessor = 4\r\n    Professor = 5 ) : ");
            AcademicTitle academicTitle = (AcademicTitle)Academicnum;
            DateTime hireDate = DateTime.Now;
            int departmentId = chekInt("Enter departmentId :");
            var result = _instructorService.AddInstructor(fullName, email, phone, academicTitle, hireDate, departmentId);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("Instructor added successfully");
            }
            else
            {
                ShowErrorMessage($"Can't Add Instructor : {result.ErrorMessage}");
            }
        }
        void ListAllInstructor()
        {
            Console.Clear();
            PrintTitle("Get All Instructor ");
            var instructorList = _instructorService.GetAllInstructors();
            if (!instructorList.Any())
            {
                ShowWarningMessage("No Instructor found.");
                return;
            }
            string[] header = { "ID ", "FullName", "Email", "AcademicTitle", "HireDate", "DepartmentId", "Status" };
            var rows = instructorList.Select(e => new string[]
            {
                $"[white]{e.InstructorId}[/]",
                $"[cyan]{e.FullName}[/]",
                $"[green]{e.Email}[/]",
                $"[white]{e.AcademicTitle}[/]",
                $"[white]{e.HireDate}[/]",
                $"[cyan]{e.Department}[/]",
                $"[green]{e.Status}[/]"
            });
            PrintTable(header, rows);
        }
        void ViewInstructor()
        {
            Console.Clear();
            PrintTitle("VIEW INSTRUCTOR DETAILS");
            int id = chekInt("Enter Instructor Id : ");
            var instructor = _instructorService.GetInstructor(id);
            var sections = _instructorService.GetSectionsByInstructor(id);
            if(instructor == null)
            {
                ShowErrorMessage("Con't found Instructor ");
                return;
            }
            string[] header = { "ID", "FullName", "Title", "Email", "DepartmentId", "Status" };
            string[][] rows = new string[][]
            { 
                new string[]{
               $"[white]{instructor.InstructorId}[/]",
               $"[yellow]{instructor.FullName}[/]",
               $"[green]{instructor.AcademicTitle}[/]",
               $"[gray]{instructor.Email}[/]",
               $"[cyan]{instructor.DepartmentId}[/]",
               $"[white]{instructor.Status}[/]"
                }
            };
            PrintTable(header, rows ,false);
            Console.WriteLine($"======Instructor: {instructor.FullName} Section's=====")  ;
            if(sections == null)
            {
                ShowErrorMessage("Con't found any section ");
                return;
            }
            string[] header2 = { "Section Id", "Section Number", "Day", "Start Time " };
            var rows2 = sections.Select(e => new string[]
            {
                $"[white]{e.SectionId}[/]",
                $"[yellow]{e.SectionNumber}[/]",
                $"[gray]{string.Join(",", e.SectionSchedules.Select(s=> s.DayOfWeek))}[/]",
                $"[cyan]{string.Join(",",e.SectionSchedules.Select(s=> s.StartTime.ToString(@"hh\:mm")))}[/]",
               
            });
            PrintTable(header2 , rows2);

        }
        void UpdateInstructors()
        {
            Console.Clear();
            PrintTitle("UPDATE INSTRUCTOR");
            int id = chekInt("Enter Instructor Id : ");

            string fullName = chekstring("Enter Full Name : ");
            string Phone = chekstring("Enter Phone Number : ");

            int Academicnum = chekInt("Enter Academic Title Num (TeachingAssistant = 1\r\n    Lecturer = 2\r\n    AssistantProfessor = 3\r\n    AssociateProfessor = 4\r\n    Professor = 5 ) : ");
            AcademicTitle academicTitle = (AcademicTitle)Academicnum;

            int departmentId = chekInt("Enter Department Id : ");

            var result = _instructorService.UpdateInstructor(id, fullName, Phone, academicTitle, departmentId);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("Update Instructors ....done");
            }
            else
            {
                ShowErrorMessage($"Can't Update Instructor{result.ErrorMessage}");
            }
        }
        void ResignInstructor()
        {
            Console.Clear();
            PrintTitle("Resign INSTRUCTOR");
            int instructorId = chekInt("Enter Instructor Id : ");
            var result = _instructorService.ResignInstructor(instructorId);
            if(result.IsSuccess)
            {
                ShowSuccessMessage("Resign Instructor done...");
            }
            else
            {
                ShowErrorMessage($"Can't Resign : {result.ErrorMessage}");
            }
        }

    }
}
