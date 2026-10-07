using System;
using University_Academic.Entities;
using University_Academic.Helper_Function;
using University_Academic.Services;
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
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                       ShowErrorMessage("Invalid choice! Please select between 1 and 6.");
                        break;
                }

            } while (isRunning);
        }

        #region UI Drawing & Formatting

        private void Print()
        {
            Console.Clear();

            // Color definitions using ANSI escape sequences
            string cyan = "\u001b[36;1m";
            string yellow = "\u001b[33;1m";
            string green = "\u001b[32;1m";
            string red = "\u001b[31;1m";
            string reset = "\u001b[0m";
            string title = "DEPARTMENT MANAGEMENT SYSTEM";

            Console.WriteLine($"{cyan}╔═════════════════════════════════════════════════════════════════════════╗{reset}");
            Console.WriteLine($"{cyan}║{reset}  {yellow}{title,-71}{reset}{cyan}║{reset}");
            Console.WriteLine($"{cyan}╠═════════════════════════════════════════════════════════════════════════╣{reset}");
            Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 1 ]{reset} Add Department       {green}[ 4 ]{reset} Update Department                   {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 2 ]{reset} View Departments     {green}[ 5 ]{reset} Delete Department                   {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 3 ]{reset} Search Department    {green}[ 6 ]{reset} View Students                       {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 7 ]{reset} View Instructors     {green}[ 8 ]{reset} View Courses                        {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}                      {red}[ 0 ]{reset} Exit / Back                                  {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
            Console.WriteLine($"{cyan}╚═════════════════════════════════════════════════════════════════════════╝{reset}");
            Console.WriteLine();
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
            int collegeId = chekInt("Enter College ID: ");

            _departmentService.AddDepartment(name, code, description, collegeId);
            ShowSuccessMessage("Department added successfully!");
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

        private void SearchDepartmentUI()
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
            var dept = _departmentService.GetDepartmentById(id);

            if (dept == null)
            {
                ShowWarningMessage("Department not found!");
                return;
            }

            string name = chekstring($"Enter New Name: ");
            string code = chekstring($"Enter New Code : ");
            string description = chekstring($"Enter New Description : ");
            int collegeId = chekInt($"Enter New College ID : ");

            _departmentService.UpdateDepartment(id, name, code, description, collegeId);
            ShowSuccessMessage("Department updated successfully!");
        }

        private void DeleteDepartmentUI()
        {
            Console.Clear();
            PrintTitle("Delete Department");
            

            int id = chekInt("Enter Department ID to delete: ");

            try
            {
                _departmentService.DeleteDepartment(id);
                ShowSuccessMessage("Department deleted successfully!");
            }
            catch (Exception ex)
            {
               
               ShowErrorMessage($"Cannot delete: {ex.Message}");
            }
        }

        #endregion
    }
}