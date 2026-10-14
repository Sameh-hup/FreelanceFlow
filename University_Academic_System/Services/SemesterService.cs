using System.Collections.Generic;
using System.Linq;
using University_Academic_System.Entities;

namespace University_Academic_System.Services
{
    public class SemesterService : ISemesterService
    {
        private static List<Semester> semesters = new List<Semester>();

        public void AddSemester(Semester semester)
        {
            semesters.Add(semester);
        }

        public List<Semester> GetAllSemesters()
        {
            return semesters;
        }

        public Semester GetSemesterById(int id)
        {
            return semesters.FirstOrDefault(s => s.Id == id);
        }

        public void ActivateSemester(int id)
        {
            var semester = GetSemesterById(id);
            if (semester != null)
            {
                semester.Status = Enums.SemesterStatus.Active;
            }
        }

        public void CompleteSemester(int id)
        {
            var semester = GetSemesterById(id);
            if (semester != null)
            {
                semester.Status = Enums.SemesterStatus.Completed;
            }
        }
    }
}