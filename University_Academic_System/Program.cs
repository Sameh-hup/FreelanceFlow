using Microsoft.EntityFrameworkCore;

using University_Academic_System.Data;
using University_Academic_System.Services;
using University_Academic_System.UI;

var options = new DbContextOptionsBuilder<UniversityDbContext>()
    .UseSqlServer(
        "Server=.;Database=UniversityAcademicDb;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

using var context = new UniversityDbContext(options);

var collegeService = new CollegeService(context);
var departmentService = new DepartmentService(context);
var main_Menu = new MainMenu(collegeService, departmentService);

main_Menu.Show();