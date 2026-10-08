using System;
using University_Academic_System.Services;
using University_Academic_System.Helper_Function;
using static University_Academic_System.Helper_Function.ConsoleHelper;
namespace University_Academic_System.UI;

    public class MainMenu
    {
        private readonly CollegeService _collegeService;
        private readonly DepartmentService _departmentService;

        public MainMenu(CollegeService collegeService, DepartmentService departmentService)
        {
            _collegeService = collegeService;
            _departmentService = departmentService;
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
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        ShowWarningMessage("\nThis module is under development...");
                        Console.ReadKey();
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                        ShowErrorMessage("\nInvalid choice! Please try again.");
                        break;
                }
            }while (isRunning);
        }

        #region UI Formatting Methods
        private void Print()
        {
            Console.Clear();

            // Color definitions using ANSI escape sequences
            string cyan = "\u001b[36;1m";
            string yellow = "\u001b[33;1m";
            string green = "\u001b[32;1m";
            string red = "\u001b[31;1m";
            string reset = "\u001b[0m";
            string title = "SMART UNIVERSITY MANAGEMENT SYSTEM";

            Console.WriteLine($"{cyan}╔═════════════════════════════════════════════════════════════════════════╗{reset}");
            Console.WriteLine($"{cyan}║{reset}  {yellow}{title,-71}{reset}{cyan}║{reset}");
            Console.WriteLine($"{cyan}╠═════════════════════════════════════════════════════════════════════════╣{reset}");
            Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 1 ]{reset} College Management       {green}[ 7 ]{reset} Classroom Management            {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 2 ]{reset} Department Management    {green}[ 8 ]{reset} Course Section Management       {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 3 ]{reset} Student Management       {green}[ 9 ]{reset} Course Registration             {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 4 ]{reset} Instructor Management    {green}[ 10 ]{reset} Grade Management               {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 5 ]{reset} Course Management        {green}[ 11 ]{reset} Academic Reports               {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {green}[ 6 ]{reset} Semester Management      {green}[ 12 ]{reset} Search System                  {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}    {red}[ 0 ]{reset} Exit                                                          {cyan}║{reset}");
            Console.WriteLine($"{cyan}║{reset}                                                                         {cyan}║{reset}");
            Console.WriteLine($"{cyan}╚═════════════════════════════════════════════════════════════════════════╝{reset}");
            Console.WriteLine();
        }
        #endregion
    }
