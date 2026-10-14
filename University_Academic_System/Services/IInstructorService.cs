using University_Academic_System.Entities;
using University_Academic_System.Enums;

namespace University_Academic_System.Services;

public interface IInstructorService
{
    Task<bool> AddInstructorAsync(Instructor instructor);
    Task<List<Instructor>> GetAllInstructorsAsync();
    Task<Instructor?> GetInstructorByIdAsync(int id);
    Task<bool> UpdateInstructorStatusAsync(int instructorId, InstructorStatus newStatus);
}