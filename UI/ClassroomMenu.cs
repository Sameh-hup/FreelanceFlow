

using University_Academic.Entities;
using University_Academic.Services;
using static University_Academic.Helper_Function.ConsoleHelper;
namespace University_Academic.UI;

public class ClassroomMenu
{
    private readonly ClassroomService _classroomService;
    public ClassroomMenu(ClassroomService classroomService)
    {
        _classroomService = classroomService;
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
                    addClassroom();
                    break;
                case 2:
                    viewAllClassrooms();
                    break;
                case 3:
                    updateClassroom();
                    break;
                case 4:
                    deleteClassroom();
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
    void Print()
    {

        var ClassroomMenuOptions = new List<string>
            {
                "Add Classroom",            //1
                "View All Classrooms",      //2
                "Update Classroom",         //3
                "Delete Classroom"        //4
                            //0
            };
        PrintMenu("CLASSROOMS MENU", ClassroomMenuOptions);
    }

    void addClassroom()
    {
        Console.Clear();
        PrintTitle("ADD CLASSROOM");

        string building = chekstring("Enter Building Name : ");
        string roomNumber = chekstring("Enter Room Number : ");
        int capacity = chekInt("Enter Capacity (1-500): ");

        var result = _classroomService.AddClassroom(building, roomNumber, capacity);
        if (result.IsSuccess)
        {
            ShowSuccessMessage("Classroom added successfully!");

        }
        else
        {
            ShowErrorMessage(result.ErrorMessage);
        }
    }
    void viewAllClassrooms()
    {
        Console.Clear();
        PrintTitle("VIEW ALL CLASSROOMS");

        var classrooms = _classroomService.GetAllClassrooms();

        string[]header = { "Classroom ID", "Building", "Room Number", "Capacity" };
        var rows = classrooms.Select(e => new string[]
        {
            $"[white]{e.ClassroomId}[/]",
            $"[green]{e.Building}[/]",
            $"[blue]{e.RoomNumber}[/]",
            $"[yellow]{e.Capacity}[/]"
        });
        PrintTable(header, rows);
    }
    void updateClassroom()
    {
        Console.Clear();
        PrintTitle("UPDATE CLASSROOM");

        int classroomId = chekInt("Enter Classroom ID to Update: ");
        string building = chekstring("Enter New Building Name : ");
        string roomNumber = chekstring("Enter New Room Number : ");
        int capacity = chekInt("Enter New Capacity (1-500): ");

        
        var success = _classroomService.UpdateClassroom(classroomId, building, roomNumber, capacity);
        if(success.IsSuccess)
        {
            ShowSuccessMessage("Classroom updated successfully!");
        }
        else
        {
            ShowErrorMessage(success.ErrorMessage);
        }
    }
    void deleteClassroom()
    {
        Console.Clear();
        PrintTitle("Delete Class Room ");

        int classroomId = chekInt("Enter ClassRoom Id : ");

        var isDelete = _classroomService.DeleteClassroom(classroomId);
        if(isDelete.IsSuccess)
        {
            ShowSuccessMessage("Classroom deleted successfully!");
        } else
            ShowErrorMessage(isDelete.ErrorMessage);
    }
}
