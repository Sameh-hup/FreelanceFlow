

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace University_Academic.Entities;

public class Classroom
{
    public int ClassroomId { get; set; }
    public string Building {  get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<CourseSection> CourseSections { get; set; } = new();

}
