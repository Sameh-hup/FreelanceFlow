using System;
using University_Academic_System.Entities;
using University_Academic_System.Services;

namespace University_Academic_System.UI
{
    public class ClassroomMenu
    {
        private readonly IClassroomService _classroomService;

        public ClassroomMenu(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Classroom Management Menu ---");
                Console.WriteLine("1. Add New Classroom");
                Console.WriteLine("2. View All Classrooms");
                Console.WriteLine("3. Back to Main Menu");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    var classroom = new Classroom();
                    Console.Write("Enter Classroom ID: ");
                    classroom.Id = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter Building Name: ");
                    classroom.Building = Console.ReadLine() ?? "";
                    Console.Write("Enter Room Number: ");
                    classroom.RoomNumber = Console.ReadLine() ?? "";
                    Console.Write("Enter Capacity (1 - 500): ");
                    classroom.Capacity = int.Parse(Console.ReadLine() ?? "50");

                    _classroomService.AddClassroom(classroom);
                    Console.WriteLine("Classroom added successfully!");
                }
                else if (choice == "2")
                {
                    var classrooms = _classroomService.GetAllClassrooms();
                    Console.WriteLine("\n--- Classrooms List ---");
                    foreach (var c in classrooms)
                    {
                        Console.WriteLine($"ID: {c.Id}, Building: {c.Building}, Room: {c.RoomNumber}, Capacity: {c.Capacity}");
                    }
                }
                else if (choice == "3")
                {
                    break;
                }
            }
        }
    }
}