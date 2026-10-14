using System.ComponentModel.DataAnnotations;

namespace University_Academic_System.Entities;

public class CourseSection
{
    [Key]
    public int SectionId { get; set; }

    public int CourseId { get; set; }
    public int InstructorId { get; set; }
    public Instructor? Instructor { get; set; }
    public int SemesterId { get; set; }
    public int ClassroomId { get; set; }

    public int SectionNumber { get; set; }
    public int Capacity { get; set; }
}