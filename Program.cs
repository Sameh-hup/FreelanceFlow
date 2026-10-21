using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Services;
using University_Academic.UI;
using static University_Academic.DataSeeder;

var options = new DbContextOptionsBuilder<UniversityDbContext>()
    .UseSqlServer(
        "Server=.;Database=UniversityAcademicDb;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

await context.Database.MigrateAsync();
SeedData(context);

var collegeService = new CollegeService(context);
var departmentService = new DepartmentService(context);
var classroomService = new ClassroomService(context);
var semesterService = new SemesterService(context);
var courseService = new CourseService(context);
var instructor = new InstructorService(context);
var section =new SectionService(context);
var student =new StudentService(context);
var grade =new GradeServices(context);
var gpa = new GpaService(context);
var registration =new RegistrationService(context,gpa);



var main_Menu = new MainMenu(collegeService
    ,departmentService
    ,classroomService
    , semesterService
    , courseService
    ,instructor
    ,section
    ,student
    ,registration
    ,grade
    ,gpa);
main_Menu.Show();
