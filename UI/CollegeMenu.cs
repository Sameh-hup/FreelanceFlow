
using Spectre.Console;
using University_Academic.Helper_Function;
using University_Academic.Services;
using static University_Academic.Helper_Function.ConsoleHelper;
namespace University_Academic.UI
{
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
                int input = ReadInt("\n Choose : ");
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

        private void Print()
        {
            Console.Clear();
            var CollegeMenuOptions = new List<string>
                    {
                        " View Colleges",
                        "Add College",
                        "Search College",
                        "Update College",
                        "Delete College",
                        "View Departments"
                    };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", CollegeMenuOptions);
        }


        // 1. Display All


        void GetAllCollegesUI()
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

            string name = chekstring("Enter College Name: ");
            string code = chekstring("Enter College Code: ");
            string description = chekstring("Enter Description: ");

            var result = _collegeService.AddCollege(name, code, description);
            if (result.IsSuccess)
            {
                ShowSuccessMessage("College added successfully!");

            }
            else
            {
                ShowErrorMessage($"Failed to add college: {result.ErrorMessage}");
            }
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
            string name = ConsoleHelper.chekstring("Enter New College Name: ");
            string code = ConsoleHelper.chekstring("Enter New College Code: ");
            string description = ConsoleHelper.chekstring("Enter New Description: ");

            var updated = _collegeService.UpdateCollege(id, name, code, description);

            if (updated.IsSuccess)
            {
                ShowSuccessMessage("College updated successfully!");
            }
            else
            {
                ShowErrorMessage($"Failed to update college: {updated.ErrorMessage}");
            }
        }

        // 5. Delete
        private void DeleteCollegeUI()
        {
            Console.Clear();
            PrintTitle("DELETE COLLEGE");

            int id =chekInt("Enter College ID to delete: ");

            var deleted = _collegeService.DeleteCollege(id);

            if (deleted.IsSuccess)
            {
                ShowSuccessMessage("College deleted successfully!");
            }
            else
            {
                ShowErrorMessage($"Cannot delete college: {deleted.ErrorMessage}");
            }
        }
       

        // View Departments for a specific College
        private void ViewCollegeDepartments()
        {
            Console.Clear();
            PrintTitle("VIEW COLLEGE DEPARTMENTS");
            Console.WriteLine();

            int id = chekInt(" Enter College ID:");

            var college  = _collegeService.GetCollegeById(id);
            if (college.Data == null)
            {
                ShowErrorMessage(college.ErrorMessage);
                return;
            }
            var results = _collegeService.GetDepartmentsByCollege(id);
            if (!results.Any())
            {

                ShowWarningMessage($"No departments found for {college.Data.Name}");
                return;
            }
            AnsiConsole.MarkupLine($"\n[bold yellow]::: [/] [green]{college.Data.Name}[/]\n");
            string[] headers = { "Dept ID", "Department Name", "Code" };
            var rows = results.Select(e => new string[]
            {
                $"[white]{e.DepartmentId}[/]",
                $"[green]{e.Name}[/]",
                $"[cyan]{e.Code}[/]"
            }).ToList();
            PrintTable(headers, rows);

        }
    }
}