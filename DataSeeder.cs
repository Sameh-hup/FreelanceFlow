using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;

namespace University_Academic
{
    public static class DataSeeder
    {
        public static void SeedData(UniversityDbContext context)
        {
            // تأكيد إنشاء قاعدة البيانات
            context.Database.EnsureCreated();

            // 1. ==================== الكليات والأقسام (Colleges & Departments) ====================
            if (!context.Colleges.Any())
            {
                var fcit = new College
                {
                    Name = "Faculty of Computers and Information",
                    Code = "FCIT",
                    Description = "Leading faculty in computing, artificial intelligence and software engineering."
                };

                var eng = new College
                {
                    Name = "Faculty of Engineering",
                    Code = "ENG",
                    Description = "Excellence in civil, electrical, and systems engineering."
                };

                var bus = new College
                {
                    Name = "Faculty of Business Administration",
                    Code = "BUS",
                    Description = "Modern business, accounting, and financial management education."
                };

                // أقسام كلية الحاسبات
                var csDept = new Department { Name = "Computer Science", Code = "CS", Description = "Theoretical and applied computer science.", College = fcit };
                var itDept = new Department { Name = "Information Technology", Code = "IT", Description = "Networks, infrastructure and cyber security.", College = fcit };
                var isDept = new Department { Name = "Information Systems", Code = "IS", Description = "Enterprise systems and business intelligence.", College = fcit };
                var sweDept = new Department { Name = "Software Engineering", Code = "SWE", Description = "Modern software architecture and lifecycle.", College = fcit };

                // أقسام كلية التجارة
                var accDept = new Department { Name = "Accounting", Code = "ACC", Description = "Financial and managerial accounting.", College = bus };
                var baDept = new Department { Name = "Business Administration", Code = "BA", Description = "Management, leadership, and marketing.", College = bus };

                context.Colleges.AddRange(fcit, eng, bus);
                context.Departments.AddRange(csDept, itDept, isDept, sweDept, accDept, baDept);
                context.SaveChanges();
            }

            // جلب الأقسام للاستخدام في العلاقات اللاحقة
            var cs = context.Departments.First(d => d.Code == "CS");
            var it = context.Departments.First(d => d.Code == "IT");
            var isDep = context.Departments.First(d => d.Code == "IS");
            var swe = context.Departments.First(d => d.Code == "SWE");

            // 2. ==================== القاعات والمعامل (Classrooms) ====================
            if (!context.Classrooms.Any())
            {
                var classrooms = new List<Classroom>
                {
                    new() { Building = "Main Building", RoomNumber = "Hall A", Capacity = 150 },
                    new() { Building = "Main Building", RoomNumber = "Hall B", Capacity = 120 },
                    new() { Building = "Main Building", RoomNumber = "Room 101", Capacity = 60 },
                    new() { Building = "Main Building", RoomNumber = "Room 102", Capacity = 60 },
                    new() { Building = "Main Building", RoomNumber = "Room 201", Capacity = 50 },
                    new() { Building = "IT Building", RoomNumber = "Lab 1", Capacity = 30 },
                    new() { Building = "IT Building", RoomNumber = "Lab 2", Capacity = 35 },
                    new() { Building = "IT Building", RoomNumber = "Network Lab", Capacity = 25 },
                    new() { Building = "Engineering Hall", RoomNumber = "Auditorium 1", Capacity = 200 },
                    new() { Building = "Business Block", RoomNumber = "Hall C", Capacity = 80 }
                };
                context.Classrooms.AddRange(classrooms);
                context.SaveChanges();
            }

            // 3. ==================== الفصول الدراسية (Semesters) ====================
            if (!context.Semesters.Any())
            {
                var semesters = new List<Semester>
                {
                    new() { Name = SemesterName.Fall, AcademicYear = "2024/2025", StartDate = new DateTime(2024, 9, 15), EndDate = new DateTime(2025, 1, 20), Status = SemesterStatus.Completed },
                    new() { Name = SemesterName.Spring, AcademicYear = "2024/2025", StartDate = new DateTime(2025, 2, 10), EndDate = new DateTime(2025, 6, 20), Status = SemesterStatus.Completed },
                    new() { Name = SemesterName.Fall, AcademicYear = "2025/2026", StartDate = new DateTime(2025, 9, 15), EndDate = new DateTime(2026, 1, 25), Status = SemesterStatus.Active }
                };
                context.Semesters.AddRange(semesters);
                context.SaveChanges();
            }

            // 4. ==================== المحاضرون (Instructors) ====================
            if (!context.Instructors.Any())
            {
                var instructors = new List<Instructor>
                {
                    // قسم CS
                    new() { FullName = "Dr. Mohamed Ahmed Ali", Email = "m.ahmed@univ.edu", Phone = "01011112222", AcademicTitle = AcademicTitle.Professor, HireDate = new DateTime(2015, 9, 1), Status = InstructorStatus.Active, DepartmentId = cs.DepartmentId },
                    new() { FullName = "Dr. Sara Hassan Ibrahim", Email = "sara.hassan@univ.edu", Phone = "01022223333", AcademicTitle = AcademicTitle.AssociateProfessor, HireDate = new DateTime(2018, 2, 1), Status = InstructorStatus.Active, DepartmentId = cs.DepartmentId },
                    new() { FullName = "Dr. Mahmoud Youssef", Email = "m.youssef@univ.edu", Phone = "01033334444", AcademicTitle = AcademicTitle.Lecturer, HireDate = new DateTime(2020, 9, 1), Status = InstructorStatus.Active, DepartmentId = cs.DepartmentId },
                    new() { FullName = "Eng. Omar Tarek", Email = "omar.tarek@univ.edu", Phone = "01044445555", AcademicTitle = AcademicTitle.TeachingAssistant, HireDate = new DateTime(2022, 10, 1), Status = InstructorStatus.Active, DepartmentId = cs.DepartmentId },

                    // قسم IT
                    new() { FullName = "Dr. Mona El-Sayed", Email = "mona.elsayed@univ.edu", Phone = "01112223334", AcademicTitle = AcademicTitle.Professor, HireDate = new DateTime(2016, 9, 1), Status = InstructorStatus.Active, DepartmentId = it.DepartmentId },
                    new() { FullName = "Dr. Khaled Abdelrahman", Email = "khaled.a@univ.edu", Phone = "01123334445", AcademicTitle = AcademicTitle.AssociateProfessor, HireDate = new DateTime(2019, 3, 1), Status = InstructorStatus.Active, DepartmentId = it.DepartmentId },
                    new() { FullName = "Eng. Nourhan Mostafa", Email = "nourhan.m@univ.edu", Phone = "01134445556", AcademicTitle = AcademicTitle.TeachingAssistant, HireDate = new DateTime(2023, 1, 15), Status = InstructorStatus.Active, DepartmentId = it.DepartmentId },

                    // قسم IS
                    new() { FullName = "Dr. Hany Mostafa Kamel", Email = "hany.kamel@univ.edu", Phone = "01211114444", AcademicTitle = AcademicTitle.Professor, HireDate = new DateTime(2014, 9, 1), Status = InstructorStatus.Active, DepartmentId = isDep.DepartmentId },
                    new() { FullName = "Dr. Rania Adel", Email = "rania.adel@univ.edu", Phone = "01222225555", AcademicTitle = AcademicTitle.Lecturer, HireDate = new DateTime(2021, 9, 1), Status = InstructorStatus.Active, DepartmentId = isDep.DepartmentId },

                    // قسم SWE
                    new() { FullName = "Dr. Tamer Samir", Email = "tamer.samir@univ.edu", Phone = "01233336666", AcademicTitle = AcademicTitle.AssociateProfessor, HireDate = new DateTime(2017, 8, 1), Status = InstructorStatus.Active, DepartmentId = swe.DepartmentId },
                    new() { FullName = "Dr. Adel Mansour (On Leave)", Email = "adel.m@univ.edu", Phone = "01244447777", AcademicTitle = AcademicTitle.Lecturer, HireDate = new DateTime(2018, 9, 1), Status = InstructorStatus.OnLeave, DepartmentId = cs.DepartmentId }
                };
                context.Instructors.AddRange(instructors);
                context.SaveChanges();
            }

            // 5. ==================== المقررات والمتطلبات (Courses & Prerequisites) ====================
            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    // سنة أولى
                    new() { Code = "CS101", Name = "Introduction to Programming", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "MATH101", Name = "Calculus I", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "IT101", Name = "Introduction to Information Technology", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = it.DepartmentId },
                    new() { Code = "IS101", Name = "Principles of Information Systems", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = isDep.DepartmentId },

                    // سنة ثانية
                    new() { Code = "CS102", Name = "Object-Oriented Programming (OOP)", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "CS201", Name = "Data Structures & Algorithms", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "IT201", Name = "Database Systems I", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = it.DepartmentId },
                    new() { Code = "IT202", Name = "Computer Networks Fundamentals", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = it.DepartmentId },

                    // سنة ثالثة
                    new() { Code = "CS301", Name = "Algorithms Analysis & Design", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "CS302", Name = "Operating Systems", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "IT301", Name = "Advanced Database Systems", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = it.DepartmentId },
                    new() { Code = "SWE301", Name = "Software Engineering Principles", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = swe.DepartmentId },

                    // سنة رابعة
                    new() { Code = "CS401", Name = "Artificial Intelligence & Machine Learning", CreditHours = 3, Level = AcademicLevel.FourthYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "CS499", Name = "Graduation Capstone Project", CreditHours = 6, Level = AcademicLevel.FourthYear, DepartmentId = cs.DepartmentId },
                    new() { Code = "IT401", Name = "Cloud Computing & DevOps", CreditHours = 3, Level = AcademicLevel.FourthYear, DepartmentId = it.DepartmentId }
                };

                context.Courses.AddRange(courses);
                context.SaveChanges();

                // ربط المتطلبات السابقة (شجرة متسلسلة ونظيفة بدون Cycles)
                var cs101 = context.Courses.First(c => c.Code == "CS101");
                var cs102 = context.Courses.First(c => c.Code == "CS102");
                var cs201 = context.Courses.First(c => c.Code == "CS201");
                var cs301 = context.Courses.First(c => c.Code == "CS301");
                var cs302 = context.Courses.First(c => c.Code == "CS302");
                var it201 = context.Courses.First(c => c.Code == "IT201");
                var it301 = context.Courses.First(c => c.Code == "IT301");
                var cs401 = context.Courses.First(c => c.Code == "CS401");

                var prereqs = new List<CoursePrerequisite>
                {
                    new() { CourseId = cs102.CourseId, PrerequisiteCourseId = cs101.CourseId }, // CS102 يشترط CS101
                    new() { CourseId = cs201.CourseId, PrerequisiteCourseId = cs102.CourseId }, // CS201 يشترط CS102
                    new() { CourseId = cs301.CourseId, PrerequisiteCourseId = cs201.CourseId }, // CS301 يشترط CS201
                    new() { CourseId = cs302.CourseId, PrerequisiteCourseId = cs201.CourseId }, // CS302 يشترط CS201
                    new() { CourseId = it301.CourseId, PrerequisiteCourseId = it201.CourseId }, // IT301 يشترط IT201
                    new() { CourseId = cs401.CourseId, PrerequisiteCourseId = cs301.CourseId }  // CS401 يشترط CS301
                };

                context.CoursePrerequisites.AddRange(prereqs);
                context.SaveChanges();
            }

            // 6. ==================== الطلاب (Students) ====================
            if (!context.Students.Any())
            {
                var students = new List<Student>
                {
                    // طلاب متفوقون
                    new() { UniversityId = "ST0001", FullName = "Ahmed Mahmoud Hassan", Email = "ahmed.m@std.univ.edu", Phone = "01099887766", DateOfBirth = new DateTime(2003, 4, 15), Gender = Gender.Male, AcademicLevel = AcademicLevel.FourthYear, EnrollmentDate = new DateTime(2021, 9, 20), DepartmentId = cs.DepartmentId, Status = StudentStatus.Active },
                    new() { UniversityId = "ST0002", FullName = "Mariam Adel Youssef", Email = "mariam.a@std.univ.edu", Phone = "01088776655", DateOfBirth = new DateTime(2004, 8, 22), Gender = Gender.Female, AcademicLevel = AcademicLevel.ThirdYear, EnrollmentDate = new DateTime(2022, 9, 20), DepartmentId = cs.DepartmentId, Status = StudentStatus.Active },
                    
                    // طلاب متوسطون
                    new() { UniversityId = "ST0003", FullName = "Kareem Tarek Mostafa", Email = "kareem.t@std.univ.edu", Phone = "01077665544", DateOfBirth = new DateTime(2004, 11, 5), Gender = Gender.Male, AcademicLevel = AcademicLevel.ThirdYear, EnrollmentDate = new DateTime(2022, 9, 20), DepartmentId = it.DepartmentId, Status = StudentStatus.Active },
                    new() { UniversityId = "ST0004", FullName = "Nadine Ibrahim Samy", Email = "nadine.i@std.univ.edu", Phone = "01066554433", DateOfBirth = new DateTime(2005, 3, 10), Gender = Gender.Female, AcademicLevel = AcademicLevel.SecondYear, EnrollmentDate = new DateTime(2023, 9, 20), DepartmentId = cs.DepartmentId, Status = StudentStatus.Active },
                    
                    // طالب تحت الإنذار الأكاديمي Probation (لاختبار شرط الـ 12 ساعة AC-10)
                    new() { UniversityId = "ST0005", FullName = "Hossam Amr Galal", Email = "hossam.a@std.univ.edu", Phone = "01055443322", DateOfBirth = new DateTime(2004, 1, 18), Gender = Gender.Male, AcademicLevel = AcademicLevel.SecondYear, EnrollmentDate = new DateTime(2023, 9, 20), DepartmentId = cs.DepartmentId, Status = StudentStatus.Active },

                    // طالب موقوف عن الدراسة Suspended (لاختبار رفض التسجيل AC-07)
                    new() { UniversityId = "ST0006", FullName = "Ziad Walid Nabil", Email = "ziad.w@std.univ.edu", Phone = "01044332211", DateOfBirth = new DateTime(2003, 6, 30), Gender = Gender.Male, AcademicLevel = AcademicLevel.ThirdYear, EnrollmentDate = new DateTime(2022, 9, 20), DepartmentId = it.DepartmentId, Status = StudentStatus.Suspended },

                    // طالب جديد مستجد سنة أولى
                    new() { UniversityId = "ST0007", FullName = "Yasmine Hany Soliman", Email = "yasmine.h@std.univ.edu", Phone = "01033221100", DateOfBirth = new DateTime(2006, 9, 12), Gender = Gender.Female, AcademicLevel = AcademicLevel.FirstYear, EnrollmentDate = new DateTime(2025, 9, 15), DepartmentId = cs.DepartmentId, Status = StudentStatus.Active },
                    new() { UniversityId = "ST0008", FullName = "Mostafa Raafat Ezzat", Email = "mostafa.r@std.univ.edu", Phone = "01022110099", DateOfBirth = new DateTime(2006, 2, 8), Gender = Gender.Male, AcademicLevel = AcademicLevel.FirstYear, EnrollmentDate = new DateTime(2025, 9, 15), DepartmentId = it.DepartmentId, Status = StudentStatus.Active }
                };

                context.Students.AddRange(students);
                context.SaveChanges();
            }

            // 7. ==================== فتح الشعب والجداول (Sections & Schedules) ====================
            var activeSem = context.Semesters.First(s => s.Status == SemesterStatus.Active);
            var completedSem1 = context.Semesters.First(s => s.AcademicYear == "2024/2025" && s.Name == SemesterName.Fall);
            var completedSem2 = context.Semesters.First(s => s.AcademicYear == "2024/2025" && s.Name == SemesterName.Spring);

            var room101 = context.Classrooms.First(c => c.RoomNumber == "Room 101");
            var hallA = context.Classrooms.First(c => c.RoomNumber == "Hall A");
            var lab1 = context.Classrooms.First(c => c.RoomNumber == "Lab 1");

            var drAhmed = context.Instructors.First(i => i.FullName.Contains("Mohamed Ahmed"));
            var drSara = context.Instructors.First(i => i.FullName.Contains("Sara Hassan"));
            var drMona = context.Instructors.First(i => i.FullName.Contains("Mona El-Sayed"));

            var cs101Crs = context.Courses.First(c => c.Code == "CS101");
            var cs102Crs = context.Courses.First(c => c.Code == "CS102");
            var it201Crs = context.Courses.First(c => c.Code == "IT201");
            var cs201Crs = context.Courses.First(c => c.Code == "CS201");

            if (!context.CourseSections.Any())
            {
                // شعب الفصل السابق خريف 2024
                var secOld1 = new CourseSection { CourseId = cs101Crs.CourseId, InstructorId = drAhmed.InstructorId, SemesterId = completedSem1.SemesterId, ClassroomId = hallA.ClassroomId, SectionNumber = 1, Capacity = 100 };
                var secOld2 = new CourseSection { CourseId = it201Crs.CourseId, InstructorId = drMona.InstructorId, SemesterId = completedSem1.SemesterId, ClassroomId = room101.ClassroomId, SectionNumber = 1, Capacity = 50 };

                // شعب الفصل السابق ربيع 2024
                var secOld3 = new CourseSection { CourseId = cs102Crs.CourseId, InstructorId = drSara.InstructorId, SemesterId = completedSem2.SemesterId, ClassroomId = hallA.ClassroomId, SectionNumber = 1, Capacity = 80 };

                // شعب الفصل الحالي النشط خريف 2025
                var secActive1 = new CourseSection { CourseId = cs101Crs.CourseId, InstructorId = drAhmed.InstructorId, SemesterId = activeSem.SemesterId, ClassroomId = hallA.ClassroomId, SectionNumber = 1, Capacity = 100 };
                var secActive2 = new CourseSection { CourseId = cs102Crs.CourseId, InstructorId = drSara.InstructorId, SemesterId = activeSem.SemesterId, ClassroomId = room101.ClassroomId, SectionNumber = 1, Capacity = 50 };
                var secActive3 = new CourseSection { CourseId = cs201Crs.CourseId, InstructorId = drAhmed.InstructorId, SemesterId = activeSem.SemesterId, ClassroomId = room101.ClassroomId, SectionNumber = 1, Capacity = 40 };

                // شعبة بسعة محدودة جداً لاختبار اكتمال السعة (AC-11)
                var secFullTest = new CourseSection { CourseId = it201Crs.CourseId, InstructorId = drMona.InstructorId, SemesterId = activeSem.SemesterId, ClassroomId = lab1.ClassroomId, SectionNumber = 1, Capacity = 2 };

                context.CourseSections.AddRange(secOld1, secOld2, secOld3, secActive1, secActive2, secActive3, secFullTest);
                context.SaveChanges();

                // مواعيد الشعب الأسبوعية
                var schedules = new List<SectionSchedule>
                {
                    new() { SectionId = secActive1.SectionId, DayOfWeek = StudyDayOfWeek.Sunday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0) },
                    new() { SectionId = secActive1.SectionId, DayOfWeek = StudyDayOfWeek.Tuesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0) },

                    new() { SectionId = secActive2.SectionId, DayOfWeek = StudyDayOfWeek.Sunday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 30, 0) },
                    new() { SectionId = secActive2.SectionId, DayOfWeek = StudyDayOfWeek.Tuesday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 30, 0) },

                    new() { SectionId = secActive3.SectionId, DayOfWeek = StudyDayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 30, 0) },
                    new() { SectionId = secFullTest.SectionId, DayOfWeek = StudyDayOfWeek.Wednesday, StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(13, 30, 0) }
                };

                context.SectionSchedules.AddRange(schedules);
                context.SaveChanges();
            }

            // 8. ==================== التسجيلات والدرجات السابقة (Historical Enrollments & Grades) ====================
            if (!context.Enrollments.Any())
            {
                var st1 = context.Students.First(s => s.UniversityId == "ST0001");
                var st2 = context.Students.First(s => s.UniversityId == "ST0002");
                var st5 = context.Students.First(s => s.UniversityId == "ST0005");

                var secOld1 = context.CourseSections.First(s => s.SemesterId == completedSem1.SemesterId && s.Course.Code == "CS101");
                var secOld3 = context.CourseSections.First(s => s.SemesterId == completedSem2.SemesterId && s.Course.Code == "CS102");
                var secFullTest = context.CourseSections.First(s => s.SemesterId == activeSem.SemesterId && s.Capacity == 2);

                // الطالب ST0001 نجح بامتياز A في CS101 و CS102
                var enr1 = new Enrollment { StudentId = st1.StudentId, SectionId = secOld1.SectionId, RegistrationDate = new DateTime(2024, 9, 20), Status = EnrollmentStatus.Completed };
                var enr2 = new Enrollment { StudentId = st1.StudentId, SectionId = secOld3.SectionId, RegistrationDate = new DateTime(2025, 2, 15), Status = EnrollmentStatus.Completed };

                // الطالبة ST0002 نجحت بتقدير B في CS101
                var enr3 = new Enrollment { StudentId = st2.StudentId, SectionId = secOld1.SectionId, RegistrationDate = new DateTime(2024, 9, 20), Status = EnrollmentStatus.Completed };

                // الطالب ST0005 رسب في CS101 بتقدير F (مجموع 45) لاختبار سياسة الإعادة والإنذار
                var enr4 = new Enrollment { StudentId = st5.StudentId, SectionId = secOld1.SectionId, RegistrationDate = new DateTime(2024, 9, 20), Status = EnrollmentStatus.Completed };

                // ملء السكشن التجريبي لطالبين لاختبار رفض الطالب الثالث لاكتمال السعة (AC-11)
                var enrFull1 = new Enrollment { StudentId = st1.StudentId, SectionId = secFullTest.SectionId, RegistrationDate = DateTime.Now.AddDays(-5), Status = EnrollmentStatus.Registered };
                var enrFull2 = new Enrollment { StudentId = st2.StudentId, SectionId = secFullTest.SectionId, RegistrationDate = DateTime.Now.AddDays(-4), Status = EnrollmentStatus.Registered };

                context.Enrollments.AddRange(enr1, enr2, enr3, enr4, enrFull1, enrFull2);
                context.SaveChanges();

                // رصد درجات الفصول المكتملة
                var grades = new List<Grade>
                {
                    new() { EnrollmentId = enr1.EnrollmentId, Midterm = 19.5m, Practical = 19.0m, Final = 56.5m }, // Total: 95.0 -> A (4.00)
                    new() { EnrollmentId = enr2.EnrollmentId, Midterm = 18.0m, Practical = 18.5m, Final = 55.0m }, // Total: 91.5 -> A (4.00)
                    new() { EnrollmentId = enr3.EnrollmentId, Midterm = 16.0m, Practical = 16.0m, Final = 50.0m }, // Total: 82.0 -> B (3.00)
                    new() { EnrollmentId = enr4.EnrollmentId, Midterm = 8.0m, Practical = 7.0m, Final = 30.0m }   // Total: 45.0 -> F (0.00) -> Probation
                };

                context.Grades.AddRange(grades);
                context.SaveChanges();
            }
        }
    }
}