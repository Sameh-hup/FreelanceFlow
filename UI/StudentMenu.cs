using Spectre.Console;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Helper_Function;
using University_Academic.Services;
using static University_Academic.Helper_Function.ConsoleHelper;

namespace University_Academic.UI;

public class StudentMenu
{
    private readonly StudentService _studentService;

    public StudentMenu(StudentService studentService)
    {
        _studentService = studentService;
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
                    AddNewStudent();
                    break;
                case 2:
                     viewStudentDetails();
                    break;
                case 3:
                    ListAllStudents();
                    break;
                case 4:
                     SearchStudents();
                    break;
                case 5:
                    SuspendStudent();
                    break;
                case 6:
                    ActivateStudent();
                    break;
                case 7:
                    WithdrawStudent();
                    break;
                case 0:
                    isRunning = false;
                    break;
                default:
                    ShowErrorMessage("Invalid choice! Please select between 1 and 7.");
                    break;
            }

        } while (isRunning);
    }
    private void Print()
    {
        Console.Clear();
        var StudentMenuOptions = new List<string>
                {
                    "Add Student",
                    "View Student",
                    "List All Students",
                    "Search Student",
                    "Suspend Student",
                    "Activate Student",
                    "Withdraw Student",
                   
                };

        PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", StudentMenuOptions);
    }
    void AddNewStudent()
    {
        Console.Clear();
        PrintTitle("Add Student");
        string fullName = chekstring("Enter Full Name : ");
        string email = chekstring("Enter Email :");
        string phone = chekstring("Enter Phone : ");
        DateTime dateOfBirth = DateTime.Parse(chekstring("Enter DateOfBirth (yyyy-MM-dd): "));
        int genderNum = chekInt("Enter Gender Number ( Male =1,\r\nFemale=2):");
        Gender gender = (Gender)genderNum;
        int academicNum = chekInt("Enter AcademicLevel Number ( firstYear = 1,\r\n    SecondYear = 2,\r\n    ThirdYear = 3,\r\n    FourthYear = 4):");
        AcademicLevel academic = (AcademicLevel)genderNum;
        DateTime enrollmentDate = DateTime.Now;
        int departmentId = chekInt("Enter Department Id: ");



        var result =_studentService.AddStudent(fullName, email, phone, dateOfBirth, gender, academic, enrollmentDate, departmentId);
        if (result.IsSuccess)
        {
            ShowSuccessMessage($"Add Student {fullName} Done ....");
        }
        else
        {
            ShowErrorMessage($"Error:{result.ErrorMessage}");
        }
    }
    void ListAllStudents()
    {
        Console.Clear();
        PrintTitle("List All Students ");
        var students = _studentService.GetStudents();
        if(students.Count <= 0)
        {
            ShowErrorMessage("Can't found any student ");
            return;
        }
        string[] header = { "Student ID ", "University ID", "Name", "Level", "Department", "Email", "Status" };
        var rows = students.Select(e => new string[]
        {
            $"[white]{e.StudentId}[/]",
            $"[green]{e.UniversityId}[/]",
            $"[cyan]{e.FullName}[/]",
            $"[yellow]{e.AcademicLevel}[/]",
            $"[cyan]{e.Department.Name}[/]",
            $"[cyan]{e.Email}[/]",
            PrintStatus(e),
            
        });
        PrintTable(header, rows,rowSeparators:true);
    }
    string PrintStatus(Student s)
    {
        if (s.Status == StudentStatus.Active)
        {
            return "[green]Active[/]";
        }
        else if (s.Status == StudentStatus.Suspended)
        {
            return "[grey]Suspended[/]";
        }
        else if (s.Status == StudentStatus.Graduated)
        {
            return "[cyan]Graduated[/]";
        }
        else
        {
            return "[red]Withdrawn[/]";
        }
    }

    void viewStudentDetails()
    {
        Console.Clear();
        PrintTitle("View Student Details");
        int studentId;
        var students = _studentService.GetStudents();
        var result = GetSelected(students, c => c.StudentId, c => c.FullName, "Student");
        if (result.IsSuccess) studentId = result.Data;
        else { ShowErrorMessage(result.ErrorMessage); return; }
        var student =_studentService.GetStudentById(studentId);
        var enrollments =_studentService.GetStudentEnrollment(studentId);
        if(student==null)
        {
            ShowErrorMessage("Can't Found Student ");
            return;
        }
        Console.Clear();
        string[] header = { "Student Id", "University Id", "Full Name", "Level", "Department", "Status", "Email" };
        string[][] rows = new string[][]
        {
                new string[]{
               $"[white]{student.StudentId}[/]",
               $"[yellow]{student.UniversityId}[/]",
               $"[yellow]{student.FullName}[/]",
               $"[green]{student.AcademicLevel}[/]",
               $"[gray]{student.Department.Name}[/]",
               PrintStatus(student),
               $"[white]{student.Email}[/]"
                }
        };
        PrintTable(header, rows, false);
        Console.WriteLine($"======Instructor: {student.FullName} course's=====");
        if (enrollments == null)
        {
            ShowErrorMessage("No records found to display.");
            return;
        }
        string[] header2 = { "Course Id", "Course Code", "Course Name" };
        var rows2 = enrollments.Select(e => new string[]
        {
                $"[white]{e.Section.Course.CourseId}[/]",
                $"[white]{e.Section.Course.Code}[/]",
                $"[white]{e.Section.Course.Name}[/]",
        });
        PrintTable(header2, rows2);

    }
    void SearchStudents()
    {
        Console.Clear();
        PrintTitle("Search Students");
        string query = chekstring("Search Student : ");
        var students= _studentService.SearchStudents(query);
        if (students.Count <= 0)
        {
            ShowErrorMessage("Can't found any student ");
            return;
        }
        string[] header = { "Student ID ", "University ID", "Name", "Level", "Department", "Email", "Status" };
        var rows = students.Select(e => new string[]
        {
            $"[white]{e.StudentId}[/]",
            $"[green]{e.UniversityId}[/]",
            $"[cyan]{e.FullName}[/]",
            $"[yellow]{e.AcademicLevel}[/]",
            $"[cyan]{e.Department.Name}[/]",
            $"[cyan]{e.Email}[/]",
            PrintStatus(e),

        });
        PrintTable(header, rows, rowSeparators: true);

    }

    void SuspendStudent()
    {
        int studentId;
        var students = _studentService.GetStudents();
        var result = GetSelected(students, c => c.StudentId, c => c.Status.ToString(), "Student","Status");
        if (result.IsSuccess) studentId = result.Data;
        else { ShowErrorMessage(result.ErrorMessage); return; }
        var done = _studentService.SuspendStudent(studentId);
        if (done.IsSuccess)
        {
            ShowSuccessMessage("Suspend Student done...");
        }
        else
        {
            ShowErrorMessage(done.ErrorMessage); return;
        }
    }
    void ActivateStudent()
    {
        int studentId;
        var students = _studentService.GetStudents();
        var result = GetSelected(students, c => c.StudentId, c => c.Status.ToString(), "Student","Status");
        if (result.IsSuccess) studentId = result.Data;
        else { ShowErrorMessage(result.ErrorMessage); return; }
        var done = _studentService.ActivateStudent(studentId);
        if (done.IsSuccess)
        {
            ShowSuccessMessage("Activate Student done...");
        }
        else
        {
            ShowErrorMessage(done.ErrorMessage); return;
        }
    }
    void WithdrawStudent()
    {
        int studentId;
        var students = _studentService.GetStudents();
        var result = GetSelected(students, c => c.StudentId, c => c.Status.ToString(), "Student","Status");
        if (result.IsSuccess) studentId = result.Data;
        else { ShowErrorMessage(result.ErrorMessage); return; }
        var done = _studentService.WithdrawStudent(studentId);
        if (done.IsSuccess)
        {
            ShowSuccessMessage("Withdraw Student done...");
        }
        else
        {
            ShowErrorMessage(done.ErrorMessage); return;
        }
    }


}
