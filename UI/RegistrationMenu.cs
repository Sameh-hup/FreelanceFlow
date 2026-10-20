using University_Academic.Entities;
using University_Academic.Services;
using static System.Collections.Specialized.BitVector32;
using static University_Academic.Helper_Function.ConsoleHelper;

namespace University_Academic.UI
{
    public class RegistrationMenu
    {
        private readonly RegistrationService _registrationService;

        public RegistrationMenu(RegistrationService registrationService)
        {
            _registrationService = registrationService;
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
                        RegisterCourseForStudent();
                        break;
                    case 2:
                         DropCourse();
                        break;
                    case 3:
                        ViewStudentCurrentSchedule();
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
                "Register Course for Student",
                "Drop Course",
                "View Student CurrentSchedule",
                
            };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", CourseMenuOptions);
        }

        void RegisterCourseForStudent()
        {
            Console.Clear();
            PrintTitle("Register Course For Student ");
            int studentId;
            int semesterId;

            var students = _registrationService.GetStudents();
            var result = GetSelected(students, c => c.StudentId, c => c.FullName, "Student");
            if (result.IsSuccess) studentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            Console.Clear();
            PrintTitle("Register Course For Student ");

            var semesters = _registrationService.GetAvailableSemester();
            var result2 = GetSelected(semesters, c => c.SemesterId, c => c.Name.ToString(), "Semester");
            if (result2.IsSuccess) semesterId = result2.Data;
            else { ShowErrorMessage(result2.ErrorMessage); return; }

            var AvailableSection = _registrationService.GetAvailableSectionsForStudent(studentId, semesterId);
            if (AvailableSection == null) { ShowErrorMessage("Can't found any Sections"); return; }

            Console.Clear();
            PrintTitle("Register Course For Student ");

            string[] header = { "Section Id", "Section No", "Course Code", "Capacity", "Enrolled", "Time & Days" };
            var rows = AvailableSection.Select(e => new string[]
            {
                $"[white]{e.SectionId}[/]",
                $"[yellow]{e.SectionNumber}[/]",
                $"[cyan]{e.Course.Code}[/]",
                $"[cyan]{e.Capacity}[/]",
                $"[magenta]{e.Enrollments?.Count ?? 0}[/]",
                $"[gray]{string.Join(", ", e.SectionSchedules.Select(s => $"{s.DayOfWeek} {s.StartTime:hh\\:mm}"))}[/]"
            });

            PrintTable(header, rows,false);

            int sectionId = chekInt("\nEnter Section Id : ");
            var done = _registrationService.RegisterStudent(studentId, sectionId);

            int currentCreditHours = _registrationService.GetCurrentSemesterCreditHours(studentId, semesterId);
            if (done.IsSuccess)
            {
                ShowSuccessMessage($"Register Student{studentId} to Section{sectionId} done...\n Student Current Credit Hours : {currentCreditHours}");
            }
            else
            {
                ShowErrorMessage(done.ErrorMessage);
            }

        }
        void DropCourse()
        {
            Console.Clear();
            PrintTitle("Drop Course");
            int studentId;
            int semesterId;

            var students = _registrationService.GetStudents();
            var result = GetSelected(students, c => c.StudentId, c => c.FullName, "Student");
            if (result.IsSuccess) studentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            Console.Clear();
            PrintTitle("Drop Course");

            var semesters = _registrationService.GetAvailableSemester();
            var result2 = GetSelected(semesters, c => c.SemesterId, c => c.Name.ToString(), "Semester");
            if (result2.IsSuccess) semesterId = result2.Data;
            else { ShowErrorMessage(result2.ErrorMessage); return; }

            var studentEnrollments= _registrationService.GetStudentCurrentEnrollments(studentId, semesterId);
            if (studentEnrollments == null) { ShowErrorMessage("Can't found any Courses"); return; }

            Console.Clear();
            PrintTitle("Drop Course");
            string[] header = { "Section Id", "Section No", "Course Code", "Time & Days" };
            var rows = studentEnrollments.Select(e => new string[]
            {
                $"[white]{e.SectionId}[/]",
                $"[yellow]{e.Section.SectionNumber}[/]",
                $"[cyan]{e.Section.Course.Code}[/]",
                $"[gray]{string.Join(", ", e.Section.SectionSchedules.Select(s => $"{s.DayOfWeek} {s.StartTime:hh\\:mm}"))}[/]"
            });
            PrintTable(header, rows, false);
            int sectionId = chekInt("\nEnter Section Id : ");

            var done =_registrationService.DropCourse(studentId, sectionId);
            if (done.IsSuccess)
            {
                ShowSuccessMessage($"Drop Student{studentId} from Section{sectionId} done...");
            }
            else
            {
                ShowErrorMessage(done.ErrorMessage);
            }

        }

        void ViewStudentCurrentSchedule()
        {
            Console.Clear();
            PrintTitle("View Student Current Schedule");
            int studentId;
            int semesterId;

            var students = _registrationService.GetStudents();
            var result = GetSelected(students, c => c.StudentId, c => c.FullName, "Student");
            if (result.IsSuccess) studentId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            Console.Clear();
            PrintTitle("View Student Current Schedule");
            var semesters = _registrationService.GetAvailableSemester();
            var result2 = GetSelected(semesters, c => c.SemesterId, c => c.Name.ToString(), "Semester");
            if (result2.IsSuccess) semesterId = result2.Data;
            else { ShowErrorMessage(result2.ErrorMessage); return; }

            var weeklySchedule = _registrationService.GetStudentWeeklySchedule(studentId, semesterId);
            if (weeklySchedule == null) { ShowErrorMessage("Can't found any Schedule"); return; }

            Console.Clear();
            PrintTitle("View Student Current Schedule");
            string[] header = { "Course Code", "Course Name", "Section No", "Day", "Start Time ", "End Time ", "Class Room " };
            var rows = weeklySchedule.Select(e => new string[]
            {
                $"[white]{e.CourseCode}[/]",
                $"[yellow]{e.CourseName}[/]",
                $"[cyan]{e.SectionNumber}[/]",
                $"[white]{e.Day}[/]",
                $"[magenta]{e.StartTime}[/]",
                $"[yellow]{e.EndTime}[/]",
                $"[cyan]{e.Classroom}[/]",
            });
            PrintTable(header, rows);
        }


    }
}
