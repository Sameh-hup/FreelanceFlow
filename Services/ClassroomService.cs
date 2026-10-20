
using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Helper_Function;
using static University_Academic.Helper_Function.ServiceResult;

namespace University_Academic.Services
{
    public class ClassroomService
    {
        private readonly UniversityDbContext _context;

        public ClassroomService(UniversityDbContext dbContext)
        {
            _context = dbContext;
        }

        public ServiceResult AddClassroom(string building,string roomNumber ,int capacity)
        {
            var existingClassroom = _context.Classrooms.FirstOrDefault(c => c.Building == building && c.RoomNumber == roomNumber);
            if (existingClassroom != null)
            {
                return Failure("Classroom already exists.");
            }
            if(capacity <= 0 || capacity > 500)
            {
                return Failure("Capacity must be between 1 and 500.");
            }
            var classroom = new Classroom
            {
                Building = building,
                RoomNumber = roomNumber,
                Capacity = capacity
            };
            _context.Classrooms.Add(classroom);
            _context.SaveChanges();
            return Success();
        }
        public ServiceResult UpdateClassroom(int classroomId, string building, string roomNumber, int capacity)
        {
            var classroom = _context.Classrooms.Find(classroomId);
            if(classroom == null)
            {
                return Failure("Classroom not found.");
            }
            if (capacity <= 0 || capacity > 500)
            {
                return Failure("Capacity must be between 1 and 500.");
            }
            classroom.Building = building;
            classroom.RoomNumber = roomNumber;
            classroom.Capacity = capacity;
            _context.SaveChanges();
            return Success();
        }
        public List<Classroom> GetAllClassrooms()
        {
            return _context.Classrooms.ToList();
        }
        public ServiceResult DeleteClassroom(int classroomId)
        {
            var classroom =_context.Classrooms.Find(classroomId);
            if( classroom == null )return Failure("Classroom not found.");
            bool isUsed =_context.CourseSections.Any(e=>e.ClassroomId == classroomId);
            if( isUsed )
            {
                return Failure("Cannot delete classroom because it is currently used in course sections.");
            }
            _context.Classrooms.Remove(classroom);
            _context.SaveChanges();
            return Success();
        }
    }
}
