using University_Academic_System.Services;

namespace University_Academic_System.UI;

public class InstructorMenu
{
    private readonly InstructorService _instructorService;

    public InstructorMenu(InstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("          Instructors Menu (Menu 4)     ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. View All Instructors");
            Console.WriteLine("2. Back to Main Menu");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();
                Console.WriteLine("--- Faculty Members List ---");
                var instructors = await _instructorService.GetAllInstructorsAsync();

                foreach (var inst in instructors)
                {
                    Console.WriteLine($"ID: {inst.InstructorId} | Name: {inst.FullName} | Email: {inst.Email} | Status: {inst.Status}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                break;
            }
        }
    }
}