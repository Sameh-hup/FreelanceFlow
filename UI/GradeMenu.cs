using Spectre.Console;
using University_Academic.Entities;
using University_Academic.Enums;
using University_Academic.Services;

using static University_Academic.Helper_Function.ConsoleHelper;

namespace University_Academic.UI;

public class GradeMenu
{
    private readonly GradeServices _gradeServices;
    private readonly GpaService _gpaService;

    public GradeMenu(GradeServices gradeServices, GpaService gpaService)
    {
        _gradeServices = gradeServices;
        _gpaService = gpaService;
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
                    RecordGrade();
                    break;
                case 2:
                    UpdateGrade();
                    break;
                case 3:
                    ViewSectionGrades();
                    break;
                case 4:
                    CalculateSemesterGPA();
                    break;
                case 5:
                    CalculateCGPA();
                    break;
                case 6:
                    StudentTranscript();
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
        var GradMenuOptions = new List<string>
        {
            "Record Grade",
            "Update Grade",
            "View Section Grades",
            "Calculate Student Semester GPA",
            "Calculate CGPA",
            "Student Transcript"
        };

        PrintMenu("SMART UNIVERSITY MANAGEMENT SYSTEM", GradMenuOptions);
    }

    void RecordGrade()
    {
        Console.Clear();
        PrintTitle("Record Grade");
        int enrollmentId;

        var enrollments = _gradeServices.GetAllEnrollments();
        var result = GetSelected(enrollments, e => e.EnrollmentId, e => $"{e.Student.FullName,-20} | {e.Section.Course.Name}", "Enrollment", $"{"Student Name",-20} | Course Name ");
        if (result.IsSuccess) { enrollmentId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        decimal midterm = (decimal)chekdouble("Enter Midterm Grade :");
        decimal practical = (decimal)chekdouble("Enter Practical Grade :");
        decimal final = (decimal)chekdouble("Enter Final Grade :");
        var done = _gradeServices.RecordGrade(enrollmentId, midterm, practical, final);
        if (done.IsSuccess)
        {
            ShowSuccessMessage(done.SuccessMessage);
        }
        else { ShowErrorMessage(done.ErrorMessage); return; }

    }
    void UpdateGrade()
    {
        Console.Clear();
        PrintTitle("Update Grade");
        int gradId;

        var grades = _gradeServices.GetAllGrades();
        var result = GetSelected(grades, e => e.GradeId, e => $"{e.Enrollment.Student.FullName,-20} | {e.Enrollment.Section.Course.Name}", "Enrollment", $"{"Student Name",-20} | Course Name ");
        if (result.IsSuccess) { gradId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        decimal midterm = (decimal)chekdouble("Enter Midterm Grade :");
        decimal practical = (decimal)chekdouble("Enter Practical Grade :");
        decimal final = (decimal)chekdouble("Enter Final Grade :");
        var done = _gradeServices.UpdateGrade(gradId, midterm, practical, final);
        if (done.IsSuccess)
        {
            ShowSuccessMessage(done.SuccessMessage);
        }
        else { ShowErrorMessage(done.ErrorMessage); return; }
    }
    void ViewSectionGrades()
    {
        Console.Clear();
        PrintTitle("View Section Grades ");
        int sectionId;
        var sections = _gradeServices.GetAllSection();
        var result = GetSelected(sections, e => e.SectionId, e => e.Course.Code, "Section", "CorseCode");
        if (result.IsSuccess) { sectionId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        var grades = _gradeServices.GetGradesBySection(sectionId);
        if (grades == null)
        {
            ShowErrorMessage("Con't found any grades ");
            return;
        }
        Console.Clear();
        PrintTitle("View Section Grades ");

        string[] header = { "Grade Id", "EnrollmentID", "Student", "Course", "Midterm", "Practical", "Final", "Total", "GradPoint" };
        var rows = grades.Select(e => new string[]
        {
            $"[white]{e.GradeId}[/]",
            $"[yellow]{e.EnrollmentId}[/]",
            $"[cyan]{e.Enrollment.Student.FullName}[/]",
            $"[cyan]{e.Enrollment.Section.Course.Name}[/]",
            $"[yellow]{e.Midterm}[/]",
            $"[yellow]{e.Practical}[/]",
            $"[yellow]{e.Final}[/]",
            $"[white]{e.Total}[/]",
            $"[green]{e.GradePoint}[/]",
        });
        PrintTable(header, rows, rowSeparators: true);
    }
    void CalculateSemesterGPA()
    {
        Console.Clear();
        PrintTitle("Calculate Semester GPA");
        int semesterId;
        int studentId;
        var students = _gradeServices.GetAllStudent();
        var result = GetSelected(students, e => e.StudentId, e => e.FullName, "Student");
        if (result.IsSuccess) { studentId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        Console.Clear();
        PrintTitle("Calculate Semester GPA");

        var semesters = _gradeServices.GetAllSemesters();
        var result2 = GetSelected(semesters, e => e.SemesterId, e => e.Name.ToString(), "Student");
        if (result2.IsSuccess) { semesterId = result2.Data; }
        else { ShowErrorMessage(result2.ErrorMessage); return; }



        var Gpa = _gpaService.CalculateSemesterGpa(studentId, semesterId);
        if (Gpa.IsSuccess)
        {
            ShowSuccessMessage($"Student Semester GPA : {Gpa.Data}");
        }
        else
        {
            ShowErrorMessage(Gpa.ErrorMessage);
        }

    }
    void CalculateCGPA()
    {
        Console.Clear();
        PrintTitle("Cumulative Grade Point Average (GPA) and Academic Status");

        int studentId;

        var students = _gradeServices.GetAllStudent();
        var result = GetSelected(students, e => e.StudentId, e => e.FullName, "Student");
        if (result.IsSuccess) { studentId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        decimal cgpa = _gpaService.CalculateCgpa(studentId);
        int earnedHours = _gpaService.CalculateEarnedHours(studentId);
        var standing = _gpaService.GetAcademicStanding(studentId);
        bool canGraduate = _gpaService.CheckGraduationEligibility(studentId);
        var student = _gpaService.GetStudent(studentId);

        //=====================
        Console.WriteLine($" Student: {student.FullName} ({student.UniversityId})");
        Console.WriteLine($" Department: {student.Department.Name}");
        Console.WriteLine("───────────────────────────────────────────────────────────────────────────────");
        Console.WriteLine($" • Cumulative GPA (CGPA):    {cgpa:F2} / 4.00 (Includes last attempt)");
        Console.WriteLine($" • Earned Hours:             {earnedHours} credit hours");

        Console.Write(" • Academic Standing:        ");
        if (standing == AcademicStanding.Probation)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Academic Probation");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(standing);
        }
        Console.ResetColor();
        Console.WriteLine($" • Graduation Eligibility:   {(canGraduate ? "Eligible ✔ (Completed 120 hrs & GPA 2.0+)" : "Not Eligible (Requirements not met)")}");
        Console.WriteLine("───────────────────────────────────────────────────────────────────────────────");
        Pause();

    }
    void StudentTranscript()
    {
        Console.Clear();
        PrintTitle("The student’s comprehensive academic record");
        int studentId;
        var students = _gradeServices.GetAllStudent();
        var result = GetSelected(students, e => e.StudentId, e => e.FullName, "Student");
        if (result.IsSuccess) { studentId = result.Data; }
        else { ShowErrorMessage(result.ErrorMessage); return; }

        var transcript = _gpaService.GetStudentTranscript(studentId);


        //====================================
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("                         OFFICIAL STUDENT TRANSCRIPT                          ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine($" Student Name   : {transcript.StudentName} ({transcript.UniversityId})");
        Console.WriteLine($" Department     : {transcript.DepartmentName}");
        Console.WriteLine($" Cumulative GPA : {transcript.CumulativeGpa:F2} / 4.00 | Earned Hours: {transcript.TotalEarnedHours} hrs");
        Console.WriteLine($" Standing       : {transcript.Standing} | Graduation Status: {(transcript.IsEligibleForGraduation ? "Eligible ✔" : "Enrolled")}");
        Console.WriteLine(new string('─', 79));
        if (!transcript.SemesterRecords.Any())
        {
            ShowErrorMessage("\nNo completed semester records found for this student.");
            return;
        }




        foreach (var sem in transcript.SemesterRecords)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n▶ Semester: {sem.SemesterName} ({sem.AcademicYear})  ---  SGPA: {sem.SemesterGpa:F2}");
            Console.ResetColor();
            string[] header = { "Course Code", "Course Name", "Credits", "Total", "Grade", "Points", "Status" };
            
            var rows = sem.Courses.Select(e => new string[]
            {

                $"[white]{e.CourseCode}[/]",
                $"[yellow]{e.CourseName}[/]",
                $"[cyan]{e.CreditHours}[/]",
                $"[cyan]{e.TotalScore}[/]",
                $"[cyan]{e.LetterGrade}[/]",
                $"[cyan]{e.GradePoint}[/]",
                e.IsPassed ? "[green]Passed[/]" : "[red]Failed[/]"
        });

            PrintTable(header, rows);
        }
        
    }

}
