
using University_Academic.Enums;

namespace University_Academic.Entities
{
    public class SectionSchedule
    {
        public int ScheduleId { get; set; }
        public int SectionId { get; set; } //FK
        public CourseSection Section { get; set; } =null!;// Navigation property
        public StudyDayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
