using University_Academic_System.Entities;

namespace University_Academic_System.Services;

public interface ISectionService
{
    Task<bool> AddCourseSectionAsync(CourseSection section);
    Task<(bool Success, string Message)> AddSectionScheduleAsync(SectionSchedule schedule);
    Task<List<CourseSection>> GetAllSectionsAsync();
}