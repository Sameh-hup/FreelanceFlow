
using Spectre.Console;
using University_Academic_System.Helper_Function;
using University_Academic_System.Services;
using static University_Academic_System.Helper_Function.ConsoleHelper;
namespace University_Academic_System.UI;

public class CollegeMenu
{
    private readonly CollegeService _collegeService;

    public CollegeMenu(CollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    public void Show()
    {
        bool IsRunning = true;

        do
        {
            Console.Clear();
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
                    GetAllCollegesUI();
                    break;
                case 2:
                    AddCollegeUI();
                    break;
                case 3:
                    SearchCollegeUI();
                    break;
                case 4:
                    UpdateCollegeUI();
                    break;
                case 5:
                    DeleteCollegeUI();
                    break;
                case 6:
                    ViewCollegeDepartments();
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

    //-------------------------------------------------------------------
    private void Print()
    {
        Console.Clear();

        // Color definitions using ANSI escape sequences
        string cyan = "\u001b[36;1m";
        string yellow = "\u001b[33;1m";
        string green = "\u001b[32;1m";
        string red = "\u001b[31;1m";
        string reset = "\u001b[0m";
        string title = "COLLEGES MANAGEMENT SYSTEM";

        Console.WriteLine($"{cyan}╔═════════════════════════════════════════════════════════════════════════╗{reset}");
        Console.WriteLine($"{cyan}║{reset}  {yellow}{title,-71}{reset}{cyan}║{reset}");
        Console.WriteLine($"{cyan}╠═════════════════════════════════════════════════════════════════════════╣{reset}");
        Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}    {green}[ 1 ]{reset} View Colleges              {green}[ 4 ]{reset} Update College                {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}    {green}[ 2 ]{reset} Add College                {green}[ 5 ]{reset} Delete College                {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}    {green}[ 3 ]{reset} Search College             {green}[ 6 ]{reset} View Departments              {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}    {red}[ 0 ]{reset}  Exit / Back                                                            {cyan}║{reset}");
        Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
        Console.WriteLine($"{cyan}╚═════════════════════════════════════════════════════════════════════════╝{reset}");
        Console.WriteLine();
    }
    //-------------------------------------------------------------------


    // 1. Display All


    private void GetAllCollegesUI()
    {
        Console.Clear();

        PrintTitle("DISPLAY ALL COLLEGES");
        var colleges = _collegeService.GetAllColleges();
        if (!colleges.Any())
        {
            ShowWarningMessage("No colleges found in the database.");
            return;
        }

        string[] header = { "ID", "College Name", "Code", "Description" };

        var rows = colleges.Select(c => new string[]
        {
            $"[white]{c.CollegeId}[/]",
            $"[green]{c.Name}[/]",
            $"[cyan]{c.Code}[/]",
            $"[grey]{c.Description ?? "N/A"}[/]"
        }).ToList();

        PrintTable(header,rows);
    }



    // 2. Add
    private void AddCollegeUI()
    {
        Console.Clear();
        PrintTitle("ADD NEW COLLEGE");
         
        string name =chekstring("Enter College Name: ");
        string code = chekstring("Enter College Code: ");
        string description = chekstring("Enter Description: ");

        _collegeService.AddCollege(name, code, description);
        AnsiConsole.WriteLine();
        ShowSuccessMessage("✓ College added successfully!");
    }

    // 3. Search
    private void SearchCollegeUI()
    {
        Console.Clear();
        PrintTitle("SEARCH COLLEGES");
        

        string term = chekstring("Enter search term (Name or Code):");

        var results = _collegeService.SearchColleges(term);
        if (!results.Any())
        {
            ShowWarningMessage(" No matching colleges found.");
            return;
        }

        string[] header = { "ID", "College Name", "Code" };
        var rows = results.Select(e => new string[]
        {
            $"[white]{e.CollegeId}[/]",
            $"[cyan]{e.Name}[/]",
            $"[green]{e.Code}[/]"
        });
        
        PrintTable(header,rows);
    }

    // 4. Update
    private void UpdateCollegeUI()
    {
        Console.Clear();
        PrintTitle("UPDATE COLLEGE");

        int id = chekInt("Enter College ID to update: ");
        var college = _collegeService.GetCollegeById(id);

        if (college == null)
        {
            ShowErrorMessage("College not found!");
            return;
        }

        AnsiConsole.MarkupLine($"[grey]Updating:[/] [yellow]{college.Name}[/]\n");

        string name = ConsoleHelper.chekstring("Enter New College Name: ");
        string code = ConsoleHelper.chekstring("Enter New College Code: ");
        string description = ConsoleHelper.chekstring("Enter New Description: ");

        bool updated = _collegeService.UpdateCollege(id, name, code, description);

        if (updated)
        {
            ShowSuccessMessage("College updated successfully!");
        }
        else
        {
            ShowErrorMessage("Failed to update college.");
        }
    }

    // 5. Delete
    private void DeleteCollegeUI()
    {
        Console.Clear();
        PrintTitle("DELETE COLLEGE");

        int id =chekInt("Enter College ID to delete: ");

        bool deleted = _collegeService.DeleteCollege(id);

        if (deleted)
        {
            ShowSuccessMessage("College deleted successfully!");
        }
        else
        {
            ShowErrorMessage("Cannot delete college! (It might not exist or contains assigned departments).");
        }
    }
   

    // View Departments for a specific College
    private void ViewCollegeDepartments()
    {
        Console.Clear();
        PrintTitle("VIEW COLLEGE DEPARTMENTS");
        Console.WriteLine();

        int id = chekInt(" Enter College ID:");

        var college = _collegeService.GetCollegeById(id);
        if (college == null)
        {
            ShowErrorMessage("College not found!");
            return;
        }
        var results = _collegeService.GetDepartmentsByCollege(id);
        if (!results.Any())
        {

            ShowWarningMessage($"No departments found for {college.Name}");
            return;
        }
        AnsiConsole.MarkupLine($"\n[bold yellow]::: [/] [green]{college.Name}[/]\n");
        string[] heders = { "Dept ID", "Department Name", "Code" };
        var rows = results.Select(e => new string[]
        {
            $"[white]{e.DepartmentId}[/]",
            $"[green]{e.Name}[/]",
            $"[cyan]{e.Code}[/]"
        }).ToList();
        PrintTable(heders, rows);

    }
}