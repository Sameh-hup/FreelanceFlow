<div align="center">

# 🎓 University Academic Management System
### *Enterprise-Grade Academic Hierarchy, Registration Pipeline & GPA Engine*

[![.NET 8 / 10](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF%20Core-Code%20First-68217A?style=for-the-badge&logo=nuget&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20N--Tier-007ACC?style=for-the-badge)](https://en.wikipedia.org/wiki/Multitier_architecture)
[![Licence](https://img.shields.io/badge/Status-Production%20Ready-success?style=for-the-badge)]()

<br/>

> **A mission-critical, enterprise academic operating system built with C#, Entity Framework Core (Code-First), and Microsoft SQL Server.**  
> Featuring an in-depth DFS Graph Cycle Detection Engine, a 1-Step Validated Course Registration Pipeline, Mathematical Collision Detection for Classrooms/Instructors, and a full Cumulative GPA Engine with Historical Retake Replacement Policy.

---

</div>

## 📑 Table of Contents
- [✨ Architectural Highlights](#-architectural-highlights)
- [🏛️ Domain Architecture & ERD](#️-domain-architecture--erd)
- [⚙️ Core Business Engines](#️-core-business-engines)
  - [1. DFS Graph Cycle Detection Engine (R8 - R10)](#1-dfs-graph-cycle-detection-engine-r8---r10)
  - [2. Multi-Dimensional Schedule Collision Engine (R15 - R19)](#2-multi-dimensional-schedule-collision-engine-r15---r19)
  - [3. The 11-Rule Registration Pipeline (R21 - R32)](#3-the-11-rule-registration-pipeline-r21---r32)
  - [4. Academic Grading & Retake GPA Engine (R33 - R43)](#4-academic-grading--retake-gpa-engine-r33---r43)
- [📊 LINQ Academic Analytics & Intelligence (FR-12)](#-linq-academic-analytics--intelligence-fr-12)
- [👥 Team Work Breakdown Structure (WBS)](#-team-work-breakdown-structure-wbs)
- [📂 Project Directory Structure](#-project-directory-structure)
- [🚀 Quick Start & Installation](#-quick-start--installation)
- [🧪 Acceptance Criteria & Automated Scenarios](#-acceptance-criteria--automated-scenarios)

---

## ✨ Architectural Highlights

* **Solid Domain-Driven Core:** 12 relational entity models mapped via Fluent API with strict referential integrity (`DeleteBehavior.Restrict` on academic hierarchies, `Cascade` on sub-slots and evaluations).
* **Graph Algorithms in Production:** Depth-First Search (DFS) algorithm with cycle prevention to secure the academic prerequisites tree from recursive or circular deadlocks.
* **Interval Math for Collisions:** Accurate algebraic time-slice overlapping algorithm: `(Start_A < End_B) && (End_A > Start_B)` safeguarding rooms, instructors, and student schedules.
* **Smart Transcript & Retake Calculations:** Dynamic GPA computation where previous failed attempts are systematically superseded by the latest course attempt in CGPA and earned hours calculations.
* **Industrial Console UX:** Built using Spectre.Console with robust error isolation, graceful input parsing (`ConsoleHelper`), formatted tables, color-coded statuses, and interactive menus.

---

## 🏛️ Domain Architecture & ERD

The system is engineered around **12 Relational Entities** backed by **11 Specialized Enumerations**:

```mermaid
erDiagram
    COLLEGE ||--o{ DEPARTMENT : "houses (1:N)"
    DEPARTMENT ||--o{ INSTRUCTOR : "employs (1:N)"
    DEPARTMENT ||--o{ STUDENT : "enrolls (1:N)"
    DEPARTMENT ||--o{ COURSE : "curates (1:N)"
    
    COURSE ||--o{ COURSE_PREREQUISITE : "requires"
    COURSE ||--o{ COURSE_PREREQUISITE : "prerequisite_for"
    COURSE ||--o{ COURSE_SECTION : "offered_as"
    
    SEMESTER ||--o{ COURSE_SECTION : "conducts"
    CLASSROOM ||--o{ COURSE_SECTION : "hosts"
    INSTRUCTOR ||--o{ COURSE_SECTION : "teaches"
    
    COURSE_SECTION ||--o{ SECTION_SCHEDULE : "scheduled_at (1:N)"
    COURSE_SECTION ||--o{ ENROLLMENT : "booked_by (1:N)"
    STUDENT ||--o{ ENROLLMENT : "registers (1:N)"
    
    ENROLLMENT ||--o| GRADE : "evaluated_by (1:1)"
```

### Relational Schema Summary:
1. **`College`**: Top-level academic institution (`Code`, `Name`, `Description`).
2. **`Department`**: Specialized academic units (`Code`, `Name`, `CollegeId`).
3. **`Student`**: Automated sequential university identification (`ST0001`, `ST0002`...), `DateOfBirth` ($\ge 16\text{ yrs}$), `AcademicLevel`, `Status`.
4. **`Instructor`**: Academic faculty members (`AcademicTitle`, `HireDate`, `Status`, `DepartmentId`).
5. **`Course`**: Curricular subjects (`Code`, `Name`, `CreditHours` [1-6], `AcademicLevel`).
6. **`CoursePrerequisite`**: Self-referential graph link (`CourseId`, `PrerequisiteCourseId`) with composite primary keys.
7. **`Semester`**: Academic terms (`Name`, `AcademicYear`, `StartDate`, `EndDate`, `Status` [Upcoming $\to$ Active $\to$ Completed]).
8. **`Classroom`**: Venues & Labs (`Building`, `RoomNumber`, `Capacity` [1-500]).
9. **`CourseSection`**: Running cohorts of a course (`SectionNumber`, `Capacity` $\le$ Room Capacity).
10. **`SectionSchedule`**: Weekly timetables (`DayOfWeek`, `StartTime`, `EndTime` with duration $\ge 45\text{ mins}$).
11. **`Enrollment`**: Student-Section binding (`RegistrationDate`, `Status` [Registered, Dropped, Completed]).
12. **`Grade`**: Academic evaluation (`Midterm` [0-20], `Practical` [0-20], `Final` [0-60], `Total` [0-100], `LetterGrade`, `GradePoint`).

---

## ⚙️ Core Business Engines

### 1. DFS Graph Cycle Detection Engine (R8 - R10)
Prevents circular dependency deadlocks across course prerequisites.
$$\text{If Course } A \to B \to \dots \to C \text{, adding } C \to A \text{ is systematically rejected.}$$

* **Algorithm:** Iterative Depth-First Search with backtracking state tracking (`Stack<int>` & `HashSet<int>`).
* **Level Integrity:** Enforces that $\text{Prerequisite.Level} \le \text{Course.Level}$ (e.g., Year 3 course cannot be a prerequisite for a Year 1 course).

```csharp
public bool HasCycle(int courseId, int newPrereqId)
{
    if (courseId == newPrereqId) return true;
    var visited = new HashSet<int>();
    var stack = new Stack<int>();
    stack.Push(newPrereqId);

    while (stack.Count > 0)
    {
        var currentId = stack.Pop();
        if (currentId == courseId) return true; // Cycle detected!

        if (visited.Add(currentId))
        {
            var parentPrereqs = _context.CoursePrerequisites
                .Where(cp => cp.CourseId == currentId)
                .Select(cp => cp.PrerequisiteCourseId);

            foreach (var prereq in parentPrereqs)
                if (!visited.Contains(prereq)) stack.Push(prereq);
        }
    }
    return false;
}
```

---

### 2. Multi-Dimensional Schedule Collision Engine (R15 - R19)
Protects against physical and temporal conflicts in lectures across the university:

$$\text{Collision}(A, B) \iff (\text{Start}_A < \text{End}_B) \land (\text{End}_A > \text{Start}_B) \quad \text{on the same day & semester}$$

* **Classroom Conflict (R17):** Guarantees that no two sections share the same hall at overlapping times.
* **Instructor Conflict (R18):** Guarantees that no instructor is booked for multiple lectures simultaneously.
* **Internal Section Conflict (R16):** Prevents self-overlapping timetable slots for the same section.
* **Teaching Load Quota (R19):** Caps maximum teaching assignments at $5\text{ sections}$ per instructor per semester.

---

### 3. The 11-Rule Registration Pipeline (R21 - R32)
Every enrollment request undergoes an atomic pipeline execution:

```
[Student Request] 
       │
       ▼
 [R21: Timetable Exists?] ─────────► (No) ──► ❌ REJECT: Section has no timetable
       │ (Yes)
 [R22: Student Active?] ───────────► (No) ──► ❌ REJECT: Suspended/Withdrawn student
       │ (Yes)
 [R23: Semester Active?] ──────────► (No) ──► ❌ REJECT: Term is not Active
       │ (Yes)
 [R24: Already Enrolled?] ─────────► (Yes) ─► ❌ REJECT: Duplicate section enrollment
       │ (No)
 [R25: Duplicate Course in Term?] ─► (Yes) ─► ❌ REJECT: Already registered in another section
       │ (No)
 [R26: Already Passed Course?] ────► (Yes) ─► ❌ REJECT: Course already completed with Total >= 60
       │ (No)
 [R27: Prerequisites Passed?] ─────► (No) ──► ❌ REJECT: Missing prerequisite courses
       │ (Yes)
 [R28: Academic Level Compatible?] ► (No) ──► ❌ REJECT: Course level > Student level
       │ (Yes)
 [R29: Student Schedule Conflict?] ► (Yes) ─► ❌ REJECT: Overlaps with student's timetable
       │ (No)
 [R30: Section Capacity Available?]► (No) ──► ❌ REJECT: Section is full
       │ (Yes)
 [R31: Credit Hours Under Cap?] ───► (No) ──► ❌ REJECT: Max 18 hrs (12 hrs for Probation)
       │ (Yes)
       ▼
 [R32: Reactivate if Dropped / Create New Registration] ──► ✔️ ENROLLMENT APPROVED
```

---

### 4. Academic Grading & Retake GPA Engine (R33 - R43)

#### Unified Grading Scale:
| Total Score Range | Letter Grade | Grade Point | Academic Result |
|:---:|:---:|:---:|:---:|
| **90.00 – 100.00** | **A** | **4.00** | Pass (Excellent) |
| **85.00 – 89.99** | **B+** | **3.50** | Pass (Very Good) |
| **80.00 – 84.99** | **B** | **3.00** | Pass (Very Good) |
| **75.00 – 79.99** | **C+** | **2.50** | Pass (Good) |
| **70.00 – 74.99** | **C** | **2.00** | Pass (Good) |
| **65.00 – 69.99** | **D+** | **1.50** | Pass (Acceptable) |
| **60.00 – 64.99** | **D** | **1.00** | Pass (Acceptable) |
| **0.00 – 59.99** | **F** | **0.00** | Fail |

#### Historical Retake Replacement (Rule R39):
When a student repeats a course they previously failed, the **Latest Attempt** supersedes past attempts in Cumulative GPA (CGPA) and earned hours calculations:

$$\text{CGPA} = \frac{\sum_{c \in \text{LatestAttempts}} (\text{GradePoint}_c \times \text{CreditHours}_c)}{\sum_{c \in \text{LatestAttempts}} \text{CreditHours}_c}$$

$$\text{Academic Standing} = \begin{cases} 
\text{Excellent} & \text{CGPA} \ge 3.50 \\
\text{Very Good} & 3.00 \le \text{CGPA} < 3.50 \\
\text{Good} & 2.50 \le \text{CGPA} < 3.00 \\
\text{Pass} & 2.00 \le \text{CGPA} < 2.50 \\
\text{Probation (Warning)} & \text{CGPA} < 2.00 
\end{cases}$$

$$\text{Graduation Requirement (R43)} \iff \text{Earned Hours} \ge 120 \quad \land \quad \text{CGPA} \ge 2.00$$

## 👥 Team Work Breakdown Structure (WBS)

```
                            ┌──────────────────────────────────────────────┐
                            │      UNIVERSITY MANAGEMENT SYSTEM (CORE)     │
                            └──────────────────────┬───────────────────────┘
                                                   │
         ┌──────────────────┬──────────────────────┼──────────────────────┬──────────────────┐
         ▼                  ▼                      ▼                      ▼                  ▼
   ┌───────────┐      ┌───────────┐          ┌───────────┐          ┌───────────┐      ┌───────────┐
   │ Member 1  │      │ Member 2  │          │ Member 3  │          │ Member 4  │      │ Member 5  │
   │Colleges & │      │ Courses & │          │Instructors│          │Students & │      │  Leader & │
   │Departments│      │DFS Cycles │          │& Conflicts│          │11-R Regist│      │Integrator │
   └───────────┘      └───────────┘          └───────────┘          └───────────┘      └───────────┘
```

| Member | Focus Domain | Key Entities | Assigned Services & Menus |
|:---|:---|:---|:---|
| **Member 1** | Academic Hierarchy | `College`, `Department` | `CollegeService`, `DepartmentService`<br/>*(Menus 1 & 2)* |
| **Member 2** | Curricular Graph & DFS | `Course`, `CoursePrerequisite`, `Semester`, `Classroom` | `CourseService`, `SemesterService`, `ClassroomService`<br/>*(Menus 5, 6, 7)* |
| **Member 3** | Faculty & Timetables | `Instructor`, `CourseSection`, `SectionSchedule` | `InstructorService`, `SectionService`, `CollisionEngine`<br/>*(Menus 4 & 8)* |
| **Member 4** | Student Life & Registration | `Student`, `Enrollment` | `StudentService`, `RegistrationService`<br/>*(Menus 3 & 9)* |
| **Member 5 (Leader)** | Core Infrastructure, Grades, Analytics | `Grade`, `UniversityDbContext`, `Fluent API` | `GradeServices`, `GpaService`, `12 LINQ Reports`, `DataSeeder`<br/>*(Menus 10, 11, 12 & Master Program.cs)* |

---

## 📂 Project Directory Structure

```plaintext
University_Academic_System/
├── 📁 Configurations/          # Fluent API EntityTypeConfiguration mappings (11 Mappings)
│   ├── CollegeConfiguration.cs
│   ├── DepartmentConfiguration.cs
│   ├── CourseConfiguration.cs
│   ├── CoursePrerequisiteConfiguration.cs
│   ├── CourseSectionConfiguration.cs
│   ├── SectionScheduleConfiguration.cs
│   ├── StudentConfiguration.cs
│   ├── InstructorConfiguration.cs
│   ├── SemesterConfiguration.cs
│   ├── ClassroomConfiguration.cs
│   └── GradeConfiguration.cs
├── 📁 Data/                    # DbContext & Runtime Factory
│   ├── UniversityDbContext.cs
│   └── ApplicationDbContextFactory.cs
├── 📁 Entities/                # Pure Domain Model classes (12 Entities)
│   ├── College.cs, Department.cs, Course.cs, Student.cs, Instructor.cs...
├── 📁 Enums/                   # System-wide Enumerations (11 Enums)
│   ├── AcademicLevel.cs, SemesterStatus.cs, LetterGrade.cs, AcademicStanding.cs...
├── 📁 Helper_Function/         # Common utilities, Safe input reader & Result pattern
│   ├── ConsoleHelper.cs        # Crash-proof input validation engine
│   └── ServiceResult.cs       # Result<T> architectural pattern
├── 📁 Services/                # Business logic engines & validations
│   ├── CollegeService.cs, CourseService.cs, SectionService.cs...
│   ├── RegistrationService.cs # 11-step registration pipeline
│   ├── GradeServices.cs       # Grade evaluation and recording
│   └── GpaService.cs          # Retake-aware GPA calculator
├── 📁 UI/                      # Interactive Spectre.Console menus
│   ├── MainMenu.cs            # Orchestrator menu
│   ├── CollegeMenu.cs, DepartmentMenu.cs, CourseMenu.cs, StudentMenu.cs...
│   ├── SectionMenu.cs, RegistrationMenu.cs, GradeMenu.cs...
├── 📄 DataSeeder.cs            # Realistic automated dataset seeder
├── 📄 Program.cs               # Application entry point & DI composition
└── 📄 appsettings.json         # Connection string configuration
```

---

## 🚀 Quick Start & Installation

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
* [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Developer edition)

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Sameh-hup/FreelanceFlow.git
   cd FreelanceFlow/University_Academic_System
   ```

2. **Configure Database Connection:**
   Open `appsettings.json` and adjust the connection string to match your SQL Server instance:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=UniversityAcademicDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply Migrations & Create Database:**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

---

## 🧪 Acceptance Criteria & Automated Scenarios

The system has been tested against real-world test cases:

* ✔️ **`AC-01` Single Active Semester:** Attempting to activate a semester while another is active throws a business validation exception.
* ✔️ **`AC-02` Circular Prerequisites:** Attempting to assign `CS101` as a prerequisite for `CS102` when `CS102` already requires `CS101` is intercepted and blocked by DFS.
* ✔️ **`AC-03` Prerequisite Level Integrity:** Year 3 courses cannot be assigned as prerequisites for Year 1 courses.
* ✔️ **`AC-04` Instructor Department Alignment:** An instructor from `IT` cannot be assigned to teach a `CS` department section.
* ✔️ **`AC-05` Section Capacity Cap:** Section capacity cannot exceed the designated classroom's maximum capacity.
* ✔️ **`AC-06` Timetable Room Overlap:** Booking the same classroom at overlapping intervals triggers collision detection.
* ✔️ **`AC-07` Inactive Student Registration:** Suspended (`Suspended`) or withdrawn students are rejected immediately at pipeline Step 2.
* ✔️ **`AC-08` Missing Prerequisites:** Students attempting to register for advanced courses without passing all prerequisite courses with $\text{Total} \ge 60$ are rejected with the list of missing courses.
* ✔️ **`AC-09` Student Schedule Overlap:** Registering for two sections with conflicting times on the same day is caught and rejected.
* ✔️ **`AC-10` Academic Probation Hour Ceiling:** Students on academic probation ($\text{CGPA} < 2.00$) cannot register for more than $12\text{ credit hours}$.
* ✔️ **`AC-11` Full Section Rejection:** Enrollments exceeding section capacity are blocked.
* ✔️ **`AC-12` Drop and Reactivation:** Dropping an active course and re-enrolling reactivates the historical row without primary/unique key violations.

---

<div align="center">

Made with dedication by the **Engineering Development Team**  
*Graduation Project / Mini Project — Academic Year 2026*

</div>
