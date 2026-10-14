using Microsoft.EntityFrameworkCore;
using University_Academic_System.Data;
using University_Academic_System.Entities;

namespace University_Academic_System.Services;

public class SectionService
{
    private readonly UniversityDbContext _context;

    public SectionService(UniversityDbContext context)
    {
        _context = context;
    }

   
    public async Task<bool> AddCourseSectionAsync(CourseSection section)
    {
        try
        {
            _context.Set<CourseSection>().Add(section);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

   
    public async Task<(bool Success, string Message)> AddSectionScheduleAsync(SectionSchedule schedule)
    {
        try
        {
            // جلب السكشن المرتبط لمعرفة المحاضر والقاعة الخاصة به
            var section = await _context.Set<CourseSection>()
                .FirstOrDefaultAsync(s => s.SectionId == schedule.SectionId);

            if (section == null)
            {
                return (false, "السكشن المرتبط غير موجود!");
            }

            // جلب الجداول المسجلة مسبقاً في نفس اليوم لفحص التداخل
            var existingSchedules = await _context.Set<SectionSchedule>()
                .Include(s => s.Section)
                .Where(s => s.DayOfWeek == schedule.DayOfWeek)
                .ToListAsync();

            foreach (var existing in existingSchedules)
            {
                // التحقق من تداخل الوقت (بداية المحاضرة الجديدة تتخلل وقتاً محجوزاً)
                bool isTimeOverlapping = schedule.StartTime < existing.EndTime && schedule.EndTime > existing.StartTime;

                if (isTimeOverlapping && existing.Section != null)
                {
                    // R16 & R17: فحص تعارض المحاضر (المحاضر لديه محاضرة أخرى في نفس الوقت)
                    if (existing.Section.InstructorId == section.InstructorId)
                    {
                        return (false, "خطأ تعارض: المحاضر مشغول في سكشن آخر في نفس اليوم والوقت!");
                    }

                    // R18 & R19: فحص تعارض القاعة (القاعة محجوزة لسكشن آخر في نفس الوقت)
                    if (existing.Section.ClassroomId == section.ClassroomId)
                    {
                        return (false, "خطأ تعارض: القاعة الدراسية محجوزة بالفعل في نفس اليوم والوقت!");
                    }
                }
            }

            
            _context.Set<SectionSchedule>().Add(schedule);
            await _context.SaveChangesAsync();

            return (true, "تم إضافة الجدول بنجاح ودون أي تعارضات!");
        }
        catch (Exception ex)
        {
            return (false, "حدث خطأ أثناء الحفظ: " + ex.Message);
        }
    }

    // 3. جلب جميع السكاشن
    public async Task<List<CourseSection>> GetAllSectionsAsync()
    {
        return await _context.Set<CourseSection>()
            .Include(s => s.Instructor)
            .ToListAsync();
    }
}