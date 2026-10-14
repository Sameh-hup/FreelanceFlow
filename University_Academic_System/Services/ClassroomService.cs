using University_Academic_System.Entities;
using System.Collections.Generic;
using System.Linq;

namespace University_Academic_System.Services
{
    public class ClassroomService : IClassroomService
    {
        private static List<Classroom> classrooms = new List<Classroom>();

        public void AddClassroom(Classroom classroom)
        {
            classrooms.Add(classroom);
        }

        public List<Classroom> GetAllClassrooms()
        {
            return classrooms;
        }

        public Classroom GetClassroomById(int id)
        {
            return classrooms.FirstOrDefault(c => c.Id == id)!;
        }

        public void UpdateClassroom(Classroom classroom)
        {
            var existing = GetClassroomById(classroom.Id);
            if (existing != null)
            {
                existing.Building = classroom.Building;
                existing.RoomNumber = classroom.RoomNumber;
                existing.Capacity = classroom.Capacity;
            }
        }

        public void DeleteClassroom(int id)
        {
            var classroom = GetClassroomById(id);
            if (classroom != null)
            {
                classrooms.Remove(classroom);
            }
        }
    }
}