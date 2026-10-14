using System.Collections.Generic;
using University_Academic_System.Entities;

namespace University_Academic_System.Services
{
    public interface ISemesterService
    {
        void AddSemester(Semester semester);
        List<Semester> GetAllSemesters();
        Semester GetSemesterById(int id);
        void ActivateSemester(int id);
        void CompleteSemester(int id);
    }
}