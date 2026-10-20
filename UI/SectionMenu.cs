using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Services;
using static System.Collections.Specialized.BitVector32;
using static University_Academic.Helper_Function.ConsoleHelper;

namespace University_Academic.UI
{
    public class SectionMenu
    {
       private readonly SectionService _sectionService;

        public SectionMenu(SectionService sectionService)
        {
            _sectionService = sectionService;
        }

        public void Show()
        {
            bool IsRunning = true;

            do
            {
                Console.Clear();
                Print();

                Console.ResetColor();
                int input = ReadInt("\n Choose : ");
                if (input == -1)
                {
                    continue;
                }
                switch (input)
                {
                    case 1:
                        CreateNewCourseSection();
                        break;
                    case 2:
                        AddScheduleSlot();
                        break;
                    case 3:
                        ListSectionsBySemester();
                        break;
                    case 4:
                        ViewSectionDetailsAndSchedule();
                        break;
                    case 5:
                        UpdateCapacity();
                        break;
                    case 6:
                        DeleteSchedule();
                        break;
                    case 0:
                        IsRunning = false;
                        break;
                    default:
                        ShowErrorMessage(" Invalid option! Please select between 1 and 6.");
                        break;
                }

            } while (IsRunning);
        }

        private void Print()
        {
            Console.Clear();
            var CourseMenuOptions = new List<string>
            {
                "Create New Course Section",
                "Add Schedule Slot to Section",
                "List Sections in Semester",
                "View Section  Schedule",
                "Update Section Capacity",
                "Delete Schedule Slot"
            };

            PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", CourseMenuOptions);
        }

        void CreateNewCourseSection()
        {
            Console.Clear();
            PrintTitle("Create Section");
            int courseId;
            int semesterId;
            int instructorId;
            int classroomId;

            var courses = _sectionService.GetAvailableCourses();
            var result = GetSelected(courses, c => c.CourseId, c => c.Name , "Course");
            if (result.IsSuccess) courseId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            var semesters = _sectionService.GetAvailableSemester();
            var result2 = GetSelected(semesters, c => c.SemesterId, c => c.Name.ToString() , "Semester");
            if (result2.IsSuccess) semesterId = result2.Data;
            else { ShowErrorMessage(result2.ErrorMessage); return; }

            var instructors = _sectionService.GetAvailableInstructor();
            var result3 = GetSelected(instructors, c => c.InstructorId, c => c.FullName , "Instructor");
            if (result3.IsSuccess) instructorId = result3.Data;
            else { ShowErrorMessage(result3.ErrorMessage); return; }

            var classrooms = _sectionService.GetAvailableClassroom();
            var result4 = GetSelected(classrooms, c => c.ClassroomId, c => c.Building , "ClassRoom");
            if (result4.IsSuccess) classroomId = result4.Data;
            else { ShowErrorMessage(result4.ErrorMessage); return; }

            int capacity = chekInt("Enter Capacity :");

            var fun =_sectionService.CreateSection(courseId,instructorId,semesterId,classroomId,capacity);
            if (fun.IsSuccess) { ShowSuccessMessage("Create Section Done ...."); }
            else { ShowErrorMessage(fun.ErrorMessage); return;}

        }

        void AddScheduleSlot()
        {
            Console.Clear();
            PrintTitle("ADD SCHEDULE SLOT");
            int sectionId;
            var sections = _sectionService.GetAvailableSections();
            var result = GetSelected(sections, c => c.SectionId, c => c.SectionNumber.ToString(), "Section","Number");
            if (result.IsSuccess) sectionId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }
            int dayNum = ReadInt("(Enter Day Number Saturday = 1,\r\n    Sunday = 2,\r\n    Monday = 3,\r\n    Tuesday = 4,\r\n    Wednesday = 5,\r\n    Thursday = 6 ) : ");
            if(dayNum > 6 || dayNum < 1)
            {
                ShowErrorMessage("Invalid option! Please select between 1 and 6. ");
                return;
            }
            StudyDayOfWeek day =(StudyDayOfWeek)dayNum;
            TimeSpan startTime = TimeSpan.Parse(chekstring("Enter startTime (01:00 - 24:00 ) h :"));
            TimeSpan endTime = TimeSpan.Parse(chekstring("Enter endTime (01:00 - 24:00 ) h :"));
            var done =_sectionService.AddScheduleSlot(sectionId,day,startTime,endTime);
            if (done.IsSuccess)
            {
                ShowSuccessMessage("Add Schedule Slot done ...");
            }
            else { ShowErrorMessage(done.ErrorMessage);  }
        }

        void ListSectionsBySemester()
        {
            Console.Clear();
            PrintTitle("View Section BY Semester ");
            int semesterId;
            var semesters= _sectionService.GetAvailableSemester();
            var result = GetSelected(semesters, c => c.SemesterId, c => c.Name.ToString(), "Semester");
            if (result.IsSuccess) { semesterId = result.Data; } 
            else { ShowErrorMessage(result.ErrorMessage); return; }

            var sections = _sectionService.GetSectionsBySemester(semesterId);
            if (sections == null) { ShowErrorMessage("Can't found any Sections"); return; }

            string[] header = { "Id", "Section No", "Course Code", "Instructor", "Classroom", "Capacity", "Enrolled", "Time & Days" };
            var rows = sections.Select(e => new string[]
            {
                $"[white]{e.SectionId}[/]",
                $"[yellow]{e.SectionNumber}[/]",
                $"[cyan]{e.CourseId}[/]",
                $"[green]{e.Instructor?.FullName ?? "Not Assigned"}[/]",
                $"[cyan]{e.ClassroomId}[/]",
                $"[cyan]{e.Capacity}[/]",
                $"[magenta]{e.Enrollments?.Count ?? 0}[/]",
                $"[gray]{string.Join(", ", e.SectionSchedules.Select(s => $"{s.DayOfWeek} {s.StartTime:hh\\:mm}"))}[/]"
            });
            PrintTable(header, rows);

        }

        void ViewSectionDetailsAndSchedule()
        {
            Console.Clear();
            PrintTitle("View Section Schedule ");
            int sectionId;
            var sections = _sectionService.GetAvailableSections();
            var result = GetSelected(sections, c => c.SectionId, c => c.SectionNumber.ToString(), "Section","SectionNum");
            if (result.IsSuccess) sectionId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }
            var section = _sectionService.GetSectionDetails(sectionId);
            if (section == null)
            {
                ShowErrorMessage("Con't found Instructor ");
                return;
            }
            string[] header = { "Section ID", "SectionNumber", "Day","Time" };
            string[][] rows = new string[][]
            {
                new string[]{
                $"[white]{section.SectionId}[/]",
                $"[yellow]{section.SectionNumber}[/]",
                $"[gray]{string.Join(",", section.SectionSchedules.Select(s=> s.DayOfWeek))}[/]",
                $"[cyan]{string.Join(",",section.SectionSchedules.Select(s=> s.StartTime.ToString(@"hh\:mm")))}[/]",
                }
            };
            PrintTable(header, rows);
        }
        void UpdateCapacity()
        {
            Console.Clear();
            PrintTitle("Update Section Capacity ");
            int sectionId;
            var sections = _sectionService.GetAvailableSections();
            var result = GetSelected(sections, c => c.SectionId, c => c.SectionNumber.ToString(), "Course" ,"Number");
            if (result.IsSuccess) sectionId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            int newCapacity = chekInt("Enter New Cpacity");
            var done = _sectionService.UpdateCapacity(sectionId, newCapacity);
            if (done.IsSuccess)
            {
                ShowSuccessMessage("Update Capacity Done....");
            }
            else
            {
                ShowErrorMessage(done.ErrorMessage); return;
            }

        }
        void DeleteSchedule()
        {
            Console.Clear();
            PrintTitle("Delete Section Schedule ");
            int scheduleId;
            var schedule = _sectionService.GetAvailableSchedule();
            var result = GetSelected(schedule, c => c.ScheduleId, c => c.DayOfWeek.ToString(), "Schedule" ,"Day");
            if (result.IsSuccess) scheduleId = result.Data;
            else { ShowErrorMessage(result.ErrorMessage); return; }

            var done = _sectionService.DeleteScheduleSlot(scheduleId);
            if (done.IsSuccess)
            {
                ShowSuccessMessage("Delete Schedule Done....");
            }
            else
            {
                ShowErrorMessage(done.ErrorMessage); return;
            }
        }
    }
}
