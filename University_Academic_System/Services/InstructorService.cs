using Microsoft.EntityFrameworkCore;
using University_Academic_System.Data;
using University_Academic_System.Entities;
using University_Academic_System.Enums;

namespace University_Academic_System.Services;

public class InstructorService
{
    private readonly UniversityDbContext _context;

    public InstructorService(UniversityDbContext context)
    {
        _context = context;
    }

    // إضافة محاضر جديد
    public async Task<bool> AddInstructorAsync(Instructor instructor)
    {
        try
        {
            _context.Set<Instructor>().Add(instructor);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // جلب جميع المحاضرين مع تفاصيل القسم
    public async Task<List<Instructor>> GetAllInstructorsAsync()
    {
        return await _context.Set<Instructor>()
            .Include(i => i.Department)
            .ToListAsync();
    }

    // جلب محاضر برقم الـ ID
    public async Task<Instructor?> GetInstructorByIdAsync(int id)
    {
        return await _context.Set<Instructor>()
            .Include(i => i.Department)
            .FirstOrDefaultAsync(i => i.InstructorId == id);
    }

    // تحديث حالة المحاضر (R12)
    public async Task<bool> UpdateInstructorStatusAsync(int instructorId, InstructorStatus newStatus)
    {
        var instructor = await _context.Set<Instructor>().FindAsync(instructorId);
        if (instructor == null) return false;

        instructor.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }
}