using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Services;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ConsoleHelper;

namespace University_Academic.UI;

public class SemesterMenu
{
    private  readonly SemesterService _semesterService;

    public SemesterMenu(SemesterService semesterService)
    {
        _semesterService = semesterService;
    }
    public void Show()
    {
        bool IsRunning = true;

        do
        {
           
            Print();

            Console.ResetColor();
            int input = chekInt("\n Choose : ");
            if (input == -1)
            {
                continue;
            }
            switch (input)
            {
                case 1:
                    AddSemester();
                    break;
                case 2:
                    ViewAllSemesters();
                    break;
                case 3:
                    ActivatStatus();
                    break;
                case 4:
                    CompleteStatus();
                    break;
                case 0:
                    IsRunning = false;

                    break;
                default:
                    ShowErrorMessage(" Invalid option! Please select between 1 and 4.");
                    break;
            }

        } while (IsRunning);
    }
    void Print()
    {
        var SemesterMenuOptions = new List<string>
        {
            "Add Semester",            //1
            "View All Semesters",      //2
            "Activate Semester",         //3
            "Complete Semester",         //4
        };
        PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", SemesterMenuOptions);
    }

    void AddSemester()
    {
        Console.Clear();
        PrintTitle("Add Semester");

        int nameNum = chekInt("Enter Semester Number (Fall = 1, Spring = 2 , Summer = 3 ): ");
        SemesterName name  = (SemesterName)nameNum;

        string academicYear = chekstring("Enter Academic Year (e.g., 2023-2024): ");

        DateTime startDate  = DateTime.Parse(chekstring("Enter Start Date (yyyy-MM-dd): "));
        DateTime endDate = DateTime.Parse(chekstring("Enter End Date (yyyy-MM-dd): "));

        var result =_semesterService.AddSemester(name, academicYear, startDate, endDate);
        if (result.IsSuccess)
        {
            ShowSuccessMessage("Semester added successfully.");
        }
        else
        {
            ShowErrorMessage($"Failed to add semester: {result.ErrorMessage}");
        }
    }
    void ViewAllSemesters()
    {
        Console.Clear();
        PrintTitle("All Semesters");
        var semesters=_semesterService.GetAllSemesters();
        if (semesters.Count == 0)
        {
            ShowWarningMessage("No semesters found.");
            return;
        }
        string[]header = { "ID", "Name", "Academic Year", "Start Date", "End Date", "Status" };


       
        var rows = semesters.Select(s => new string[]
        {
            $"[white]{s.SemesterId}[/]",
            $"[green]{s.Name}[/]",
            $"[blue]{s.AcademicYear}[/]",
            $"[yellow]{s.StartDate.ToShortDateString()}[/]",
            $"[yellow]{s.EndDate.ToShortDateString()}[/]",
            PrintStatus(s)
        });

        PrintTable(header, rows);

        }
    string PrintStatus(Semester s)
    {
        if(s.Status == SemesterStatus.Active)
        {
            return "[green]Active[/]";
        }
        else if (s.Status == SemesterStatus.Completed)
        {
            return "[grey]Completed[/]";
        }
        else
        {
            return "[cyan]Upcoming[/]";
        }
    }

    void ActivatStatus()
    {
        Console.Clear();
        PrintTitle("Active Status ");

        int semesterId = chekInt("Enter SemesterId : ");

        var active = _semesterService.ActivateSemester(semesterId);
        if (active.IsSuccess)
        {
            ShowSuccessMessage("Active Semester Status ....done ");
        }
        if (!active.IsSuccess)
        {
            ShowErrorMessage($"Failed to activate semester: {active.ErrorMessage}");

        }
    }
    void CompleteStatus()
    {
        Console.Clear();
        PrintTitle("Complete Status ");

        int semesterId = chekInt("Enter SemesterId : ");

        var complete = _semesterService.CompleteSemester(semesterId);
        if(complete.IsSuccess)
        {
            ShowSuccessMessage("Complete Semester Status ....done ");
        }
        else
        {
            ShowErrorMessage($"Failed to complete semester: {complete.ErrorMessage}");
        }
    }
}
