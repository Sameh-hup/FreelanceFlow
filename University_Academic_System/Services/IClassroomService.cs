using University_Academic_System.Entities;
using System.Collections.Generic;

namespace University_Academic_System.Services
{
    public interface IClassroomService
    {
        void AddClassroom(Classroom classroom);
        List<Classroom> GetAllClassrooms();
        Classroom GetClassroomById(int id);
        void UpdateClassroom(Classroom classroom);
        void DeleteClassroom(int id);
    }
}