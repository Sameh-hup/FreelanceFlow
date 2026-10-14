using System.ComponentModel.DataAnnotations;
using University_Academic_System.Enums;

namespace University_Academic_System.Entities;

public class SectionSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    public int SectionId { get; set; }
    public CourseSection? Section { get; set; }

    public StudyDayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}