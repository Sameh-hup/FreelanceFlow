using System;
using University_Academic_System.Entities;
using University_Academic_System.Enums;
using University_Academic_System.Services;

namespace University_Academic_System.UI
{
    public class SemesterMenu
    {
        private readonly ISemesterService _semesterService;

        public SemesterMenu(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Semester Management Menu ---");
                Console.WriteLine("1. Add New Semester");
                Console.WriteLine("2. View All Semesters");
                Console.WriteLine("3. Activate Semester");
                Console.WriteLine("4. Complete Semester");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    var sem = new Semester();
                    Console.Write("Enter Semester ID: ");
                    sem.Id = int.Parse(Console.ReadLine() ?? "0");

                    Console.WriteLine("Select Semester Name (1: Fall, 2: Spring, 3: Summer): ");
                    int nameChoice = int.Parse(Console.ReadLine() ?? "1");
                    sem.Name = (SemesterName)nameChoice;

                    Console.Write("Enter Year: ");
                    sem.Year = int.Parse(Console.ReadLine() ?? "2026");

                    sem.Status = SemesterStatus.Upcoming;
                    sem.StartDate = DateTime.Now;
                    sem.EndDate = DateTime.Now.AddMonths(4);

                    _semesterService.AddSemester(sem);
                    Console.WriteLine("Semester added successfully!");
                }
                else if (choice == "2")
                {
                    var semesters = _semesterService.GetAllSemesters();
                    Console.WriteLine("\n--- Semesters List ---");
                    foreach (var s in semesters)
                    {
                        Console.WriteLine($"ID: {s.Id}, Name: {s.Name}, Year: {s.Year}, Status: {s.Status}");
                    }
                }
                else if (choice == "3")
                {
                    Console.Write("Enter Semester ID to Activate: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    _semesterService.ActivateSemester(id);
                    Console.WriteLine("Semester activated successfully!");
                }
                else if (choice == "4")
                {
                    Console.Write("Enter Semester ID to Complete: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    _semesterService.CompleteSemester(id);
                    Console.WriteLine("Semester completed successfully!");
                }
                else if (choice == "5")
                {
                    break;
                }
            }
        }
    }
}