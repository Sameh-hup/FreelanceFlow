
using University_Academic.Data;
using University_Academic.Entities;
using University_Academic.Enums;

namespace University_Academic
{
    public static class DataSeeder
    {
        public static void SeedData(UniversityDbContext context)
        {
            // منع تكرار البذر إذا كانت البيانات مكتملة بالفعل
            if (HasExistingData(context))
                return;

            // =========================================================================
            // 1. الكليات والأقسام الأكاديمية (Colleges & Departments)
            // =========================================================================
            if (!context.Colleges.Any())
            {
                var colleges = new List<College>
                {
                    // 1. كلية الحاسبات والذكاء الاصطناعي
                    new College
                    {
                        Name = "Faculty of Computers and Artificial Intelligence",
                        Code = "FCAI",
                        Description = "School of Computing, Software Engineering, and AI Systems.",
                        Departments = new List<Department>
                        {
                            new Department { Name = "Computer Science", Code = "CS", Description = "Algorithms, Software Engineering, and AI fundamentals." },
                            new Department { Name = "Information Technology", Code = "IT", Description = "Network infrastructure, Cloud Systems, and Web tech." },
                            new Department { Name = "Information Systems", Code = "IS", Description = "Database Management, ERP Systems, and Data Analytics." },
                            new Department { Name = "Artificial Intelligence", Code = "AI", Description = "Machine Learning, Computer Vision, and Robotics." },
                            new Department { Name = "Cybersecurity", Code = "CYBER", Description = "Network Security, Ethical Hacking, and Cryptography." }
                        }
                    },

                    // 2. كلية الهندسة
                    new College
                    {
                        Name = "Faculty of Engineering",
                        Code = "ENG",
                        Description = "School of Engineering Sciences and Technology.",
                        Departments = new List<Department>
                        {
                            new Department { Name = "Civil Engineering", Code = "CIVIL", Description = "Structural Engineering and Infrastructure." },
                            new Department { Name = "Mechanical Engineering", Code = "MECH", Description = "Thermodynamics and Mechanical Systems Design." },
                            new Department { Name = "Electrical Engineering", Code = "ELEC", Description = "Power Networks, Microelectronics, and Telecommunications." },
                            new Department { Name = "Architecture Engineering", Code = "ARCH", Description = "Architectural Design and Urban Planning." }
                        }
                    },

                    // 3. كلية إدارة الأعمال
                    new College
                    {
                        Name = "Faculty of Business Administration",
                        Code = "BUS",
                        Description = "School of Business, Finance, and Marketing.",
                        Departments = new List<Department>
                        {
                            new Department { Name = "Accounting", Code = "ACCT", Description = "Financial Accounting, Auditing, and Managerial Control." },
                            new Department { Name = "Finance & Banking", Code = "FIN", Description = "Financial Markets, Investment Analysis, and Banking Operations." },
                            new Department { Name = "Marketing", Code = "MKTG", Description = "Digital Marketing, Consumer Behavior, and Brand Management." },
                            new Department { Name = "Business Information Systems", Code = "BIS", Description = "E-Commerce, Data Driven Business, and ERP Solutions." }
                        }
                    },

                    // 4. كلية الطب
                    new College
                    {
                        Name = "Faculty of Medicine",
                        Code = "MED",
                        Description = "School of Medical Sciences and Clinical Studies.",
                        Departments = new List<Department>
                        {
                            new Department { Name = "General Surgery", Code = "SURG", Description = "General and Minimally Invasive Surgical Operations." },
                            new Department { Name = "Pediatrics", Code = "PED", Description = "Healthcare and Medical Management for Children." },
                            new Department { Name = "Cardiology", Code = "CARD", Description = "Cardiovascular Health and Heart Disease Treatment." },
                            new Department { Name = "Internal Medicine", Code = "INTMED", Description = "Diagnosis and Prevention of Adult Diseases." }
                        }
                    }
                };

                context.Colleges.AddRange(colleges);
                context.SaveChanges();
            }

            // تحميل الأقسام للاستخدام في العلاقات اللاحقة
            var csDept = context.Departments.First(d => d.Code == "CS");
            var itDept = context.Departments.First(d => d.Code == "IT");
            var isDept = context.Departments.First(d => d.Code == "IS");
            var aiDept = context.Departments.First(d => d.Code == "AI");
            var mechDept = context.Departments.First(d => d.Code == "MECH");
            var acctDept = context.Departments.First(d => d.Code == "ACCT");

            // =========================================================================
            // 2. القاعات الدراسية والمعامل (Classrooms)
            // =========================================================================
            if (!context.Classrooms.Any())
            {
                var classrooms = new List<Classroom>
                {
                    new Classroom { Building = "Building A", RoomNumber = "Hall A", Capacity = 100 },
                    new Classroom { Building = "Building A", RoomNumber = "Lab 1", Capacity = 30 },
                    new Classroom { Building = "Building A", RoomNumber = "Room 201", Capacity = 50 },
                    new Classroom { Building = "Building A", RoomNumber = "Room 202", Capacity = 40 },
                    new Classroom { Building = "Building B", RoomNumber = "Hall B", Capacity = 80 },
                    new Classroom { Building = "Building B", RoomNumber = "Lab 2", Capacity = 25 },
                    new Classroom { Building = "Building B", RoomNumber = "Room 301", Capacity = 60 },
                    new Classroom { Building = "Engineering Block", RoomNumber = "Eng Hall 1", Capacity = 120 },
                    new Classroom { Building = "Business Block", RoomNumber = "Bus Hall 1", Capacity = 90 }
                };

                context.Classrooms.AddRange(classrooms);
                context.SaveChanges();
            }

            var hallA = context.Classrooms.First(c => c.RoomNumber == "Hall A");
            var hallB = context.Classrooms.First(c => c.RoomNumber == "Hall B");
            var lab1 = context.Classrooms.First(c => c.RoomNumber == "Lab 1");
            var lab2 = context.Classrooms.First(c => c.RoomNumber == "Lab 2");
            var room201 = context.Classrooms.First(c => c.RoomNumber == "Room 201");
            var room202 = context.Classrooms.First(c => c.RoomNumber == "Room 202");

            // =========================================================================
            // 3. الفصول الدراسية (Semesters)
            // =========================================================================
            if (!context.Semesters.Any())
            {
                var semesters = new List<Semester>
                {
                    // فصل خريف 2025/2026 منتهي وبه درجات سابقة
                    new Semester
                    {
                        Name = SemesterName.Fall,
                        AcademicYear = "2025/2026",
                        StartDate = new DateTime(2025, 10, 1),
                        EndDate = new DateTime(2026, 1, 31),
                        Status = SemesterStatus.Completed
                    },
                    // فصل ربيع 2025/2026 هو الفصل الفعال والنشط حالياً
                    new Semester
                    {
                        Name = SemesterName.Spring,
                        AcademicYear = "2025/2026",
                        StartDate = new DateTime(2026, 2, 15),
                        EndDate = new DateTime(2026, 6, 30),
                        Status = SemesterStatus.Active
                    },
                    // فصل صيف 2025/2026 فصل قادم لتجهيز الشعب
                    new Semester
                    {
                        Name = SemesterName.Summer,
                        AcademicYear = "2025/2026",
                        StartDate = new DateTime(2026, 7, 15),
                        EndDate = new DateTime(2026, 9, 15),
                        Status = SemesterStatus.Upcoming
                    }
                };

                context.Semesters.AddRange(semesters);
                context.SaveChanges();
            }

            var fallSemester = context.Semesters.First(s => s.Name == SemesterName.Fall && s.AcademicYear == "2025/2026");
            var springSemester = context.Semesters.First(s => s.Name == SemesterName.Spring && s.AcademicYear == "2025/2026");

            // =========================================================================
            // 4. أعضاء هيئة التدريس (Instructors)
            // =========================================================================
            if (!context.Instructors.Any())
            {
                var instructors = new List<Instructor>
                {
                    // CS Department Instructors
                    new Instructor
                    {
                        FullName = "Dr. Ahmed Mahmoud",
                        Email = "ahmed.mahmoud@fcai.edu",
                        Phone = "01011112222",
                        AcademicTitle = AcademicTitle.AssociateProfessor,
                        HireDate = new DateTime(2018, 9, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Dr. Khaled Hassan",
                        Email = "khaled.hassan@fcai.edu",
                        Phone = "01022223333",
                        AcademicTitle = AcademicTitle.Professor,
                        HireDate = new DateTime(2015, 8, 15),
                        Status = InstructorStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Dr. Adel Ibrahim",
                        Email = "adel.ibrahim@fcai.edu",
                        Phone = "01033334444",
                        AcademicTitle = AcademicTitle.Lecturer,
                        HireDate = new DateTime(2020, 2, 1),
                        Status = InstructorStatus.Resigned, // مستقيل لاختبار حالات الاستقالة
                        DepartmentId = csDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Eng. Mostafa Kamel",
                        Email = "mostafa.kamel@fcai.edu",
                        Phone = "01044445555",
                        AcademicTitle = AcademicTitle.TeachingAssistant,
                        HireDate = new DateTime(2022, 10, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // IT Department Instructors
                    new Instructor
                    {
                        FullName = "Dr. Mona Khalil",
                        Email = "mona.khalil@fcai.edu",
                        Phone = "01055556666",
                        AcademicTitle = AcademicTitle.AssistantProfessor,
                        HireDate = new DateTime(2019, 1, 15),
                        Status = InstructorStatus.Active,
                        DepartmentId = itDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Eng. Tarek Sayed",
                        Email = "tarek.sayed@fcai.edu",
                        Phone = "01066667777",
                        AcademicTitle = AcademicTitle.Lecturer,
                        HireDate = new DateTime(2021, 9, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = itDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Dr. Hoda Samir",
                        Email = "hoda.samir@fcai.edu",
                        Phone = "01077778888",
                        AcademicTitle = AcademicTitle.AssociateProfessor,
                        HireDate = new DateTime(2017, 3, 1),
                        Status = InstructorStatus.OnLeave, // في إجازة
                        DepartmentId = itDept.DepartmentId
                    },

                    // IS Department Instructor
                    new Instructor
                    {
                        FullName = "Dr. Sara Fouad",
                        Email = "sara.fouad@fcai.edu",
                        Phone = "01088889999",
                        AcademicTitle = AcademicTitle.Lecturer,
                        HireDate = new DateTime(2020, 11, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = isDept.DepartmentId
                    },

                    // AI Department Instructor
                    new Instructor
                    {
                        FullName = "Dr. Yasser Zaki",
                        Email = "yasser.zaki@fcai.edu",
                        Phone = "01099990000",
                        AcademicTitle = AcademicTitle.Professor,
                        HireDate = new DateTime(2016, 5, 10),
                        Status = InstructorStatus.Active,
                        DepartmentId = aiDept.DepartmentId
                    },

                    // Other Colleges Instructors
                    new Instructor
                    {
                        FullName = "Dr. Tamer Nabil",
                        Email = "tamer.nabil@eng.edu",
                        Phone = "01111223344",
                        AcademicTitle = AcademicTitle.AssociateProfessor,
                        HireDate = new DateTime(2018, 7, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = mechDept.DepartmentId
                    },
                    new Instructor
                    {
                        FullName = "Dr. Salma Ezzat",
                        Email = "salma.ezzat@bus.edu",
                        Phone = "01222334455",
                        AcademicTitle = AcademicTitle.AssistantProfessor,
                        HireDate = new DateTime(2019, 9, 1),
                        Status = InstructorStatus.Active,
                        DepartmentId = acctDept.DepartmentId
                    }
                };

                context.Instructors.AddRange(instructors);
                context.SaveChanges();

                // تعيين رؤساء الأقسام (ChairPersons)
                var drKhaled = context.Instructors.First(i => i.Email == "khaled.hassan@fcai.edu");
                var drMona = context.Instructors.First(i => i.Email == "mona.khalil@fcai.edu");
                var drSara = context.Instructors.First(i => i.Email == "sara.fouad@fcai.edu");

                csDept.ChairPersonId = drKhaled.InstructorId;
                itDept.ChairPersonId = drMona.InstructorId;
                isDept.ChairPersonId = drSara.InstructorId;
                context.SaveChanges();
            }

            var drAhmed = context.Instructors.First(i => i.Email == "ahmed.mahmoud@fcai.edu");
            var drKhaledCS = context.Instructors.First(i => i.Email == "khaled.hassan@fcai.edu");
            var engMostafa = context.Instructors.First(i => i.Email == "mostafa.kamel@fcai.edu");
            var drMonaIT = context.Instructors.First(i => i.Email == "mona.khalil@fcai.edu");
            var engTarekIT = context.Instructors.First(i => i.Email == "tarek.sayed@fcai.edu");
            var drSaraIS = context.Instructors.First(i => i.Email == "sara.fouad@fcai.edu");
            var drYasserAI = context.Instructors.First(i => i.Email == "yasser.zaki@fcai.edu");

            // =========================================================================
            // 5. المقررات وشجرة المتطلبات السابقة (Courses & Prerequisites)
            // =========================================================================
            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    // CS Courses
                    new Course { Code = "CS101", Name = "Introduction to Programming", Description = "Basics of C# and OOP.", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = csDept.DepartmentId },
                    new Course { Code = "CS102", Name = "Data Structures", Description = "Arrays, Lists, Stacks, Queues, Trees.", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = csDept.DepartmentId },
                    new Course { Code = "CS201", Name = "Design and Analysis of Algorithms", Description = "Sorting, Searching, Greedy, DP.", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = csDept.DepartmentId },
                    new Course { Code = "CS301", Name = "Artificial Intelligence", Description = "Search algorithms, Knowledge representation, Expert systems.", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = csDept.DepartmentId },
                    new Course { Code = "CS401", Name = "Distributed Systems", Description = "RPC, Microservices, and Consensus algorithms.", CreditHours = 3, Level = AcademicLevel.FourthYear, DepartmentId = csDept.DepartmentId },

                    // IT Courses
                    new Course { Code = "IT101", Name = "Computer Networks Fundamentals", Description = "OSI, TCP/IP, Routing, Switching.", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = itDept.DepartmentId },
                    new Course { Code = "IT201", Name = "Database Systems 1", Description = "Relational Model, SQL, Normalization.", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = itDept.DepartmentId },
                    new Course { Code = "IT301", Name = "Advanced Database Systems", Description = "Indexing, Transactions, NoSQL.", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = itDept.DepartmentId },
                    new Course { Code = "IT302", Name = "Cloud Computing Architecture", Description = "IaaS, PaaS, SaaS, Docker, Kubernetes.", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = itDept.DepartmentId },

                    // IS Courses
                    new Course { Code = "IS101", Name = "Fundamentals of Information Systems", Description = "Business processes and Information management.", CreditHours = 3, Level = AcademicLevel.FirstYear, DepartmentId = isDept.DepartmentId },
                    new Course { Code = "IS201", Name = "Systems Analysis and Design", Description = "Agile, UML, System Architecture.", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = isDept.DepartmentId },
                    new Course { Code = "IS301", Name = "Enterprise Resource Planning (ERP)", Description = "ERP integration and business workflows.", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = isDept.DepartmentId },

                    // AI Courses
                    new Course { Code = "AI201", Name = "Machine Learning Fundamentals", Description = "Supervised and Unsupervised Learning.", CreditHours = 3, Level = AcademicLevel.SecondYear, DepartmentId = aiDept.DepartmentId },
                    new Course { Code = "AI301", Name = "Deep Learning & Neural Networks", Description = "CNN, RNN, Transformers.", CreditHours = 3, Level = AcademicLevel.ThirdYear, DepartmentId = aiDept.DepartmentId }
                };

                context.Courses.AddRange(courses);
                context.SaveChanges();

                // المتطلبات السابقة (Prerequisites)
                var cs101 = context.Courses.First(c => c.Code == "CS101");
                var cs102 = context.Courses.First(c => c.Code == "CS102");
                var cs201 = context.Courses.First(c => c.Code == "CS201");
                var cs301 = context.Courses.First(c => c.Code == "CS301");
                var it101 = context.Courses.First(c => c.Code == "IT101");
                var it201 = context.Courses.First(c => c.Code == "IT201");
                var it301 = context.Courses.First(c => c.Code == "IT301");
                var is101 = context.Courses.First(c => c.Code == "IS101");
                var is201 = context.Courses.First(c => c.Code == "IS201");
                var ai201 = context.Courses.First(c => c.Code == "AI201");
                var ai301 = context.Courses.First(c => c.Code == "AI301");

                var prereqs = new List<CoursePrerequisite>
                {
                    new CoursePrerequisite { CourseId = cs102.CourseId, PrerequisiteCourseId = cs101.CourseId }, // CS102 requires CS101
                    new CoursePrerequisite { CourseId = cs201.CourseId, PrerequisiteCourseId = cs102.CourseId }, // CS201 requires CS102
                    new CoursePrerequisite { CourseId = cs301.CourseId, PrerequisiteCourseId = cs201.CourseId }, // CS301 requires CS201
                    new CoursePrerequisite { CourseId = it301.CourseId, PrerequisiteCourseId = it201.CourseId }, // IT301 requires IT201
                    new CoursePrerequisite { CourseId = is201.CourseId, PrerequisiteCourseId = is101.CourseId }, // IS201 requires IS101
                    new CoursePrerequisite { CourseId = ai201.CourseId, PrerequisiteCourseId = cs101.CourseId }, // AI201 requires CS101
                    new CoursePrerequisite { CourseId = ai301.CourseId, PrerequisiteCourseId = ai201.CourseId }  // AI301 requires AI201
                };

                context.CoursePrerequisites.AddRange(prereqs);
                context.SaveChanges();
            }

            var cCS101 = context.Courses.First(c => c.Code == "CS101");
            var cCS102 = context.Courses.First(c => c.Code == "CS102");
            var cCS201 = context.Courses.First(c => c.Code == "CS201");
            var cIT101 = context.Courses.First(c => c.Code == "IT101");
            var cIT201 = context.Courses.First(c => c.Code == "IT201");
            var cIT301 = context.Courses.First(c => c.Code == "IT301");
            var cIS101 = context.Courses.First(c => c.Code == "IS101");
            var cIS201 = context.Courses.First(c => c.Code == "IS201");
            var cAI201 = context.Courses.First(c => c.Code == "AI201");

            // =========================================================================
            // 6. الشعب الدراسية والمواعيد (Sections & Schedules)
            // =========================================================================
            if (!context.CourseSections.Any())
            {
                var sections = new List<CourseSection>
                {
                    // --- شعب فصل الخريف المنتهي Fall 2025/2026 ---
                    new CourseSection
                    {
                        CourseId = cCS101.CourseId,
                        InstructorId = drAhmed.InstructorId,
                        SemesterId = fallSemester.SemesterId,
                        ClassroomId = hallA.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 60,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Saturday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cCS101.CourseId,
                        InstructorId = drKhaledCS.InstructorId,
                        SemesterId = fallSemester.SemesterId,
                        ClassroomId = room201.ClassroomId,
                        SectionNumber = 2,
                        Capacity = 45,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIT101.CourseId,
                        InstructorId = drMonaIT.InstructorId,
                        SemesterId = fallSemester.SemesterId,
                        ClassroomId = hallB.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 50,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Sunday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIT201.CourseId,
                        InstructorId = drMonaIT.InstructorId,
                        SemesterId = fallSemester.SemesterId,
                        ClassroomId = lab1.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 25,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Tuesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIS101.CourseId,
                        InstructorId = drSaraIS.InstructorId,
                        SemesterId = fallSemester.SemesterId,
                        ClassroomId = room202.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 35,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Wednesday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(13, 0, 0) }
                        }
                    },

                    // --- شعب فصل الربيع الفعال حالياً Spring 2025/2026 ---
                    new CourseSection
                    {
                        CourseId = cCS101.CourseId, // شعبة إعادة للراسبين في الخريف
                        InstructorId = drKhaledCS.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = room202.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 30,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Saturday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cCS102.CourseId, // هياكل البيانات
                        InstructorId = drAhmed.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = hallA.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 60,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Sunday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cCS102.CourseId, // شعبة ثانية لهياكل البيانات
                        InstructorId = engMostafa.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = room201.ClassroomId,
                        SectionNumber = 2,
                        Capacity = 35,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Monday, StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(14, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cCS201.CourseId, // خوارزميات
                        InstructorId = drAhmed.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = room201.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 45,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(11, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIT201.CourseId, // قواعد بيانات 1
                        InstructorId = drMonaIT.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = lab1.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 25,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Sunday, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(15, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIT301.CourseId, // قواعد بيانات متقدمة
                        InstructorId = engTarekIT.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = lab2.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 20,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Wednesday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cIS201.CourseId, // تحليل نظم
                        InstructorId = drSaraIS.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = room201.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 35,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Thursday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(13, 0, 0) }
                        }
                    },
                    new CourseSection
                    {
                        CourseId = cAI201.CourseId, // تعلم الآلة
                        InstructorId = drYasserAI.InstructorId,
                        SemesterId = springSemester.SemesterId,
                        ClassroomId = hallB.ClassroomId,
                        SectionNumber = 1,
                        Capacity = 50,
                        SectionSchedules = new List<SectionSchedule>
                        {
                            new SectionSchedule { DayOfWeek = StudyDayOfWeek.Wednesday, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(15, 0, 0) }
                        }
                    }
                };

                context.CourseSections.AddRange(sections);
                context.SaveChanges();
            }

            // =========================================================================
            // 7. الطلاب (Students) - سيناريوهات اختبار شاملة لـ AC-01 إلى AC-12
            // =========================================================================
            if (!context.Students.Any())
            {
                var students = new List<Student>
                {
                    // ST0001: طالب متفوق CS (سنة ثانية) - اجتاز CS101 ومسجل في CS102
                    new Student
                    {
                        UniversityId = "ST0001",
                        FullName = "Ahmed Ali Mohamed",
                        Email = "ahmed.ali@std.fcai.edu",
                        Phone = "01010001001",
                        DateOfBirth = new DateTime(2004, 5, 14),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0002: طالبة CS (سنة ثانية) - رسبت في CS101 وتطلب إعادتها (Retake)
                    new Student
                    {
                        UniversityId = "ST0002",
                        FullName = "Sara Ibrahim Hassan",
                        Email = "sara.ibrahim@std.fcai.edu",
                        Phone = "01010001002",
                        DateOfBirth = new DateTime(2004, 8, 20),
                        Gender = Gender.Female,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0003: طالب IT (سنة أولى) - موقوف قيده Suspended (يُمنع من التسجيل)
                    new Student
                    {
                        UniversityId = "ST0003",
                        FullName = "Mahmoud Saeed Mostafa",
                        Email = "mahmoud.saeed@std.fcai.edu",
                        Phone = "01010001003",
                        DateOfBirth = new DateTime(2005, 2, 10),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.FirstYear,
                        EnrollmentDate = new DateTime(2024, 9, 15),
                        Status = StudentStatus.Suspended,
                        DepartmentId = itDept.DepartmentId
                    },

                    // ST0004: طالب CS (سنة ثانية) - على إنذار أكاديمي Probation (معدل أقل من 2.00)
                    new Student
                    {
                        UniversityId = "ST0004",
                        FullName = "Youssef Adel Nader",
                        Email = "youssef.adel@std.fcai.edu",
                        Phone = "01010001004",
                        DateOfBirth = new DateTime(2004, 11, 5),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0005: طالبة CS (سنة رابعة) - أوائل الدفعة CGPA 3.96
                    new Student
                    {
                        UniversityId = "ST0005",
                        FullName = "Mariam Kamal Tawfik",
                        Email = "mariam.kamal@std.fcai.edu",
                        Phone = "01010001005",
                        DateOfBirth = new DateTime(2002, 3, 12),
                        Gender = Gender.Female,
                        AcademicLevel = AcademicLevel.FourthYear,
                        EnrollmentDate = new DateTime(2021, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0006: طالب مستجد CS (سنة أولى) - قام بإسقاط مقرر (Dropped)
                    new Student
                    {
                        UniversityId = "ST0006",
                        FullName = "Omar Khaled Mansour",
                        Email = "omar.khaled@std.fcai.edu",
                        Phone = "01010001006",
                        DateOfBirth = new DateTime(2005, 9, 18),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.FirstYear,
                        EnrollmentDate = new DateTime(2024, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0007: طالبة IT (سنة ثانية)
                    new Student
                    {
                        UniversityId = "ST0007",
                        FullName = "Nour Hassan Elsayed",
                        Email = "nour.hassan@std.fcai.edu",
                        Phone = "01010001007",
                        DateOfBirth = new DateTime(2004, 1, 25),
                        Gender = Gender.Female,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = itDept.DepartmentId
                    },

                    // ST0008: طالب IS (سنة ثانية)
                    new Student
                    {
                        UniversityId = "ST0008",
                        FullName = "Ziad Tarek Fathy",
                        Email = "ziad.tarek@std.fcai.edu",
                        Phone = "01010001008",
                        DateOfBirth = new DateTime(2004, 6, 30),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = isDept.DepartmentId
                    },

                    // ST0009: طالب متخرج Graduated
                    new Student
                    {
                        UniversityId = "ST0009",
                        FullName = "Kareem Farouk Hamed",
                        Email = "kareem.farouk@std.fcai.edu",
                        Phone = "01010001009",
                        DateOfBirth = new DateTime(2001, 4, 15),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.FourthYear,
                        EnrollmentDate = new DateTime(2020, 9, 15),
                        Status = StudentStatus.Graduated,
                        DepartmentId = csDept.DepartmentId
                    },

                    // ST0010: طالبة منسحبة Withdrawn
                    new Student
                    {
                        UniversityId = "ST0010",
                        FullName = "Salma Hesham Abdelrahman",
                        Email = "salma.hesham@std.fcai.edu",
                        Phone = "01010001010",
                        DateOfBirth = new DateTime(2005, 7, 11),
                        Gender = Gender.Female,
                        AcademicLevel = AcademicLevel.FirstYear,
                        EnrollmentDate = new DateTime(2024, 9, 15),
                        Status = StudentStatus.Withdrawn,
                        DepartmentId = itDept.DepartmentId
                    },

                    // ST0011: طالب IT (سنة ثالثة)
                    new Student
                    {
                        UniversityId = "ST0011",
                        FullName = "Hossam Amr Galal",
                        Email = "hossam.amr@std.fcai.edu",
                        Phone = "01010001011",
                        DateOfBirth = new DateTime(2003, 12, 1),
                        Gender = Gender.Male,
                        AcademicLevel = AcademicLevel.ThirdYear,
                        EnrollmentDate = new DateTime(2022, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = itDept.DepartmentId
                    },

                    // ST0012: طالبة AI (سنة ثانية)
                    new Student
                    {
                        UniversityId = "ST0012",
                        FullName = "Aya Sherif Badawy",
                        Email = "aya.sherif@std.fcai.edu",
                        Phone = "01010001012",
                        DateOfBirth = new DateTime(2004, 4, 18),
                        Gender = Gender.Female,
                        AcademicLevel = AcademicLevel.SecondYear,
                        EnrollmentDate = new DateTime(2023, 9, 15),
                        Status = StudentStatus.Active,
                        DepartmentId = aiDept.DepartmentId
                    }
                };

                context.Students.AddRange(students);
                context.SaveChanges();
            }

            // =========================================================================
            // 8. التسجيلات والدرجات (Enrollments & Grades)
            // =========================================================================
            if (!context.Enrollments.Any())
            {
                // جلب كائنات الطلاب
                var stAhmed = context.Students.First(s => s.UniversityId == "ST0001");
                var stSara = context.Students.First(s => s.UniversityId == "ST0002");
                var stYoussef = context.Students.First(s => s.UniversityId == "ST0004");
                var stMariam = context.Students.First(s => s.UniversityId == "ST0005");
                var stOmar = context.Students.First(s => s.UniversityId == "ST0006");
                var stNour = context.Students.First(s => s.UniversityId == "ST0007");
                var stZiad = context.Students.First(s => s.UniversityId == "ST0008");
                var stHossam = context.Students.First(s => s.UniversityId == "ST0011");
                var stAya = context.Students.First(s => s.UniversityId == "ST0012");

                // شعب الخريف
                var secFallCS101_1 = context.CourseSections.First(s => s.SemesterId == fallSemester.SemesterId && s.CourseId == cCS101.CourseId && s.SectionNumber == 1);
                var secFallCS101_2 = context.CourseSections.First(s => s.SemesterId == fallSemester.SemesterId && s.CourseId == cCS101.CourseId && s.SectionNumber == 2);
                var secFallIT101_1 = context.CourseSections.First(s => s.SemesterId == fallSemester.SemesterId && s.CourseId == cIT101.CourseId && s.SectionNumber == 1);
                var secFallIT201_1 = context.CourseSections.First(s => s.SemesterId == fallSemester.SemesterId && s.CourseId == cIT201.CourseId && s.SectionNumber == 1);
                var secFallIS101_1 = context.CourseSections.First(s => s.SemesterId == fallSemester.SemesterId && s.CourseId == cIS101.CourseId && s.SectionNumber == 1);

                // شعب الربيع
                var secSpringCS101_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cCS101.CourseId && s.SectionNumber == 1);
                var secSpringCS102_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cCS102.CourseId && s.SectionNumber == 1);
                var secSpringCS201_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cCS201.CourseId && s.SectionNumber == 1);
                var secSpringIT201_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cIT201.CourseId && s.SectionNumber == 1);
                var secSpringIT301_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cIT301.CourseId && s.SectionNumber == 1);
                var secSpringIS201_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cIS201.CourseId && s.SectionNumber == 1);
                var secSpringAI201_1 = context.CourseSections.First(s => s.SemesterId == springSemester.SemesterId && s.CourseId == cAI201.CourseId && s.SectionNumber == 1);

                // --- 1. تسجيلات فصل الخريف المنتهي Fall (Completed مع رصد الدرجات) ---
                var fallEnrollments = new List<Enrollment>
                {
                    // أحمد علي: ممتاز في CS101 و IT101 و IS101
                    new Enrollment
                    {
                        StudentId = stAhmed.StudentId,
                        SectionId = secFallCS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 18.00m, Practical = 18.00m, Final = 49.00m } // Total: 85 (B+, 3.50)
                    },
                    new Enrollment
                    {
                        StudentId = stAhmed.StudentId,
                        SectionId = secFallIT101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 19.00m, Practical = 19.00m, Final = 54.00m } // Total: 92 (A, 4.00)
                    },
                    new Enrollment
                    {
                        StudentId = stAhmed.StudentId,
                        SectionId = secFallIS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 3),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 17.00m, Practical = 16.00m, Final = 45.00m } // Total: 78 (C+, 2.50)
                    },

                    // سارة إبراهيم: رسبت في CS101 ونجحت في IT101
                    new Enrollment
                    {
                        StudentId = stSara.StudentId,
                        SectionId = secFallCS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 10.00m, Practical = 10.00m, Final = 25.00m } // Total: 45 (F, 0.00 - راسب!)
                    },
                    new Enrollment
                    {
                        StudentId = stSara.StudentId,
                        SectionId = secFallIT101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 15.00m, Practical = 14.00m, Final = 41.00m } // Total: 70 (C, 2.00)
                    },

                    // يوسف عادل: رسب في CS101 ومقبول في IS101 -> إنذار أكاديمي Probation
                    new Enrollment
                    {
                        StudentId = stYoussef.StudentId,
                        SectionId = secFallCS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 3),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 12.00m, Practical = 10.00m, Final = 36.00m } // Total: 58 (F, 0.00 - راسب!)
                    },
                    new Enrollment
                    {
                        StudentId = stYoussef.StudentId,
                        SectionId = secFallIS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 4),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 13.00m, Practical = 11.00m, Final = 38.00m } // Total: 62 (D, 1.00)
                    },

                    // مريم كمال: متفوقة جداً A في جميع المواد
                    new Enrollment
                    {
                        StudentId = stMariam.StudentId,
                        SectionId = secFallCS101_2.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 20.00m, Practical = 20.00m, Final = 58.00m } // Total: 98 (A, 4.00)
                    },
                    new Enrollment
                    {
                        StudentId = stMariam.StudentId,
                        SectionId = secFallIT201_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 19.50m, Practical = 19.50m, Final = 56.00m } // Total: 95 (A, 4.00)
                    },

                    // نور حسن
                    new Enrollment
                    {
                        StudentId = stNour.StudentId,
                        SectionId = secFallIT101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 3),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 18.00m, Practical = 17.00m, Final = 51.00m } // Total: 86 (B+, 3.50)
                    },

                    // زياد طارق
                    new Enrollment
                    {
                        StudentId = stZiad.StudentId,
                        SectionId = secFallIS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 3),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 16.00m, Practical = 15.00m, Final = 49.00m } // Total: 80 (B, 3.00)
                    },

                    // حسام عمرو
                    new Enrollment
                    {
                        StudentId = stHossam.StudentId,
                        SectionId = secFallIT201_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 4),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 17.00m, Practical = 17.00m, Final = 48.00m } // Total: 82 (B, 3.00)
                    },

                    // آية شريف
                    new Enrollment
                    {
                        StudentId = stAya.StudentId,
                        SectionId = secFallCS101_1.SectionId,
                        RegistrationDate = new DateTime(2025, 10, 2),
                        Status = EnrollmentStatus.Completed,
                        Grade = new Grade { Midterm = 19.00m, Practical = 19.00m, Final = 53.00m } // Total: 91 (A, 4.00)
                    }
                };

                context.Enrollments.AddRange(fallEnrollments);
                context.SaveChanges();

                // --- 2. تسجيلات فصل الربيع الحالي Spring 2025/2026 (Registered و Dropped) ---
                var springEnrollments = new List<Enrollment>
                {
                    // أحمد علي سجل CS102 (لأنه اجتاز CS101) وسجل IT201
                    new Enrollment
                    {
                        StudentId = stAhmed.StudentId,
                        SectionId = secSpringCS102_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },
                    new Enrollment
                    {
                        StudentId = stAhmed.StudentId,
                        SectionId = secSpringIT201_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },

                    // سارة إبراهيم تعيد دراسة CS101 في شعبة الربيع
                    new Enrollment
                    {
                        StudentId = stSara.StudentId,
                        SectionId = secSpringCS101_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 17),
                        Status = EnrollmentStatus.Registered
                    },

                    // يوسف عادل يعيد دراسة CS101
                    new Enrollment
                    {
                        StudentId = stYoussef.StudentId,
                        SectionId = secSpringCS101_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 17),
                        Status = EnrollmentStatus.Registered
                    },

                    // مريم كمال تسجل CS201 و IT301
                    new Enrollment
                    {
                        StudentId = stMariam.StudentId,
                        SectionId = secSpringCS201_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },
                    new Enrollment
                    {
                        StudentId = stMariam.StudentId,
                        SectionId = secSpringIT301_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },

                    // عمر خالد قام بإسقاط المقرر Dropped لاختبار قاعدة R32
                    new Enrollment
                    {
                        StudentId = stOmar.StudentId,
                        SectionId = secSpringCS101_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 18),
                        Status = EnrollmentStatus.Dropped
                    },

                    // نور حسن تسجل IT201
                    new Enrollment
                    {
                        StudentId = stNour.StudentId,
                        SectionId = secSpringIT201_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 17),
                        Status = EnrollmentStatus.Registered
                    },

                    // زياد طارق يسجل IS201
                    new Enrollment
                    {
                        StudentId = stZiad.StudentId,
                        SectionId = secSpringIS201_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },

                    // حسام عمرو يسجل IT301
                    new Enrollment
                    {
                        StudentId = stHossam.StudentId,
                        SectionId = secSpringIT301_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 17),
                        Status = EnrollmentStatus.Registered
                    },

                    // آية شريف تسجل CS102 و AI201
                    new Enrollment
                    {
                        StudentId = stAya.StudentId,
                        SectionId = secSpringCS102_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    },
                    new Enrollment
                    {
                        StudentId = stAya.StudentId,
                        SectionId = secSpringAI201_1.SectionId,
                        RegistrationDate = new DateTime(2026, 2, 16),
                        Status = EnrollmentStatus.Registered
                    }
                };

                context.Enrollments.AddRange(springEnrollments);
                context.SaveChanges();
            }
        }

        private static bool HasExistingData(UniversityDbContext context)
        {
            return
                context.Colleges.Any() ||
                context.Departments.Any() ||
                context.Classrooms.Any() ||
                context.Semesters.Any() ||
                context.Instructors.Any() ||
                context.Courses.Any() ||
                context.CoursePrerequisites.Any() ||
                context.CourseSections.Any() ||
                context.SectionSchedules.Any() ||
                context.Students.Any() ||
                context.Enrollments.Any() ||
                context.Set<Grade>().Any();
        }
    }
}
