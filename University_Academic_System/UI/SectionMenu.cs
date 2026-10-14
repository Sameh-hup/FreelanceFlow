using University_Academic_System.Entities;
using University_Academic_System.Enums;
using University_Academic_System.Services;

namespace University_Academic_System.UI;

public class SectionMenu
{
    private readonly SectionService _sectionService;

    public SectionMenu(SectionService sectionService)
    {
        _sectionService = sectionService;
    }

    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           Sections Menu (Menu 8)       ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. View All Course Sections");
            Console.WriteLine("2. Add Section Schedule (Conflict Checker)");
            Console.WriteLine("3. Back to Main Menu");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();
                Console.WriteLine("--- Current Sections List ---");
                var sections = await _sectionService.GetAllSectionsAsync();

                foreach (var sec in sections)
                {
                    Console.WriteLine($"Section ID: {sec.SectionId} | Section Number: {sec.SectionNumber} | Capacity: {sec.Capacity}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                Console.Clear();
                Console.WriteLine("--- Add Schedule & Check Conflicts ---");

                Console.Write("Enter SectionId: ");
                if (int.TryParse(Console.ReadLine(), out int secId))
                {
                    Console.WriteLine("Days: 1. Saturday | 2. Sunday | 3. Monday | 4. Tuesday | 5. Wednesday | 6. Thursday");
                    Console.Write("Choose day number (1 to 6): ");
                    if (int.TryParse(Console.ReadLine(), out int dayNum) && dayNum >= 1 && dayNum <= 6)
                    {
                        Console.Write("Enter Start Time (e.g., 08:30:00): ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan start))
                        {
                            Console.Write("Enter End Time (e.g., 10:00:00): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan end))
                            {
                                var schedule = new SectionSchedule
                                {
                                    SectionId = secId,
                                    DayOfWeek = (StudyDayOfWeek)dayNum,
                                    StartTime = start,
                                    EndTime = end
                                };

                                var result = await _sectionService.AddSectionScheduleAsync(schedule);
                                Console.WriteLine(result.Message);
                            }
                            else
                            {
                                Console.WriteLine("Invalid end time format!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid start time format!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid day selection!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Section ID!");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                break;
            }
        }
    }
}