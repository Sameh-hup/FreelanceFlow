# ASP.NET Core 3-Tier Architecture — Business Logic Layer (BLL) Assignment

## Project: FreelanceFlow — Freelancer & Client Management System 💼

---

## 1. Overview & Architectural Role

In FreelanceFlow, the **Business Logic Layer (`App.BLL`)** is the brain and core authority of the application. The Presentation layer (`App.Mvc`) is strictly a consumer; it is never allowed to communicate with `AppDbContext` directly, never makes financial calculations, and never determines entity states or security ownership.

As the BLL engineering team (2 developers), you are responsible for:
1. **Data Access Abstraction**: Implementing concrete repositories that encapsulate EF Core queries, eager loading, and projections.
2. **Business Rules Enforcement**: Guaranteeing all business rules (BR-01 through BR-31) and non-functional requirements (NFR-04 through NFR-14).
3. **Pure Calculation Engines**: Calculating project progress %, overdue tasks, milestone statuses, invoice statuses, and project health purely on the server.
4. **Financial & Concurrency Integrity**: Processing payments inside atomic database transactions, preventing overpayment, and resolving concurrency conflicts via `RowVersion`.
5. **Server-Side Validation**: Implementing strict FluentValidation rules for all incoming models and entities.
6. **Centralized Access Control**: Providing a unified `AccessService` that enforces strict tenancy and ownership checks.

> **Team Notice**:
> The Team Lead owns `App.DAL` (Entities, `AppDbContext`, Migrations, and Database Seeding). Do not alter entity classes or create migrations. Your work is entirely contained within `App.BLL` and its unit tests.

---

## 2. Architecture & BLL Project Structure

Your project is `App.BLL` (Class Library - .NET 10.0), referencing `App.DAL`.

```text
App.BLL
│
├── 📁 Interfaces
│   ├── IRepository.cs
│   ├── IClientRepository.cs
│   ├── IProjectRepository.cs
│   ├── IInvoiceRepository.cs
│   └── 📁 Services
│       ├── IAccessService.cs
│       ├── IAuthService.cs
│       ├── IClientService.cs
│       ├── IProjectService.cs
│       ├── ITaskService.cs
│       └── IInvoiceService.cs
│
├── 📁 Repositories
│   ├── Repository.cs                       (Generic Base Repository)
│   ├── ClientRepository.cs                 (Custom queries for Clients)
│   ├── ProjectRepository.cs                (Custom queries for Projects)
│   └── InvoiceRepository.cs                (Custom queries for Invoices & Payments)
│
├── 📁 Services
│   ├── 📁 Clock
│   │   ├── IClock.cs                       (Single source of truth for time)
│   │   └── SystemClock.cs                  (UTC & Cairo time provider)
│   ├── AccessService.cs                    (Central ownership & authorization)
│   ├── AuthService.cs                      (Authentication & Invitation token hashing)
│   ├── ClientService.cs                    (Client CRUD & Soft-Delete logic)
│   ├── ProjectService.cs                   (Project lifecycle, Progress & Health)
│   ├── TaskService.cs                      (Milestones, Tasks & Overdue engine)
│   └── InvoiceService.cs                   (Invoices, Atomic Payments & Concurrency)
│
├── 📁 Validations
│   ├── ClientValidator.cs
│   ├── ProjectValidator.cs
│   ├── TaskValidator.cs
│   ├── InvoiceValidator.cs
│   └── PaymentValidator.cs
│
└── 📁 Extensions
    └── BllServiceExtensions.cs             (AddBLL service registration)
```

---

## 3. General Rules & Engineering Guidelines

1. **Async Everywhere**:
   * Every database operation and service method returning data must be `async Task` or `async Task<T>`.
   * Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` — this causes thread starvation and deadlocks.
2. **Clock Discipline (BR-30)**:
   * **Never use `DateTime.Now` or `DateTime.UtcNow` directly.**
   * Inject `IClock` into every service requiring dates.
   * All database timestamps are saved in **UTC** (`_clock.UtcNow`).
   * All date-only comparisons (such as "Today" for overdue tasks, invoice deadlines, and project health) use `_clock.CairoToday` (Egypt Standard Time: `Africa/Cairo`).
3. **No Financial Floating-Point Math (NFR-05)**:
   * All monetary amounts must be `decimal`. Never cast money to `float` or `double`.
4. **Pure Calculation Functions (NFR-13)**:
   * Methods like `CalculateProgress`, `CalculateHealth`, `CalculateInvoiceStatus`, and `CalculateMilestoneStatus` must be pure functions (deterministic output based solely on inputs, no direct database queries inside the calculation itself).
5. **Soft-Delete Rule (BR-05, BR-26, FR-07)**:
   * When deleting a client, set `IsDeleted = true` and `DeletedAtUtc = _clock.UtcNow`. Never issue a hard delete on `Clients`.
   * All standard client queries must filter out soft-deleted clients (`!c.IsDeleted`).

---

## 4. Team Division of Responsibilities

To ensure parallel progress without merge conflicts, the BLL work is divided between two engineers:

| Role | Assigned Modules & Focus Areas | Key Interfaces & Files |
| :--- | :--- | :--- |
| **Developer 1: Core Domain & Workflow** | • Clients Management & Soft Delete<br>• Projects Lifecycle & Hierarchy<br>• Tasks & Milestones Management<br>• Progress Calculation Engine (BR-10)<br>• Overdue Tasks Detection (BR-11)<br>• Project Health State Machine (Section 8)<br>• Validators: Client, Project, Task | `IClientRepository`<br>`IProjectRepository`<br>`ClientRepository`<br>`ProjectRepository`<br>`IClientService` → `ClientService`<br>`IProjectService` → `ProjectService`<br>`ITaskService` → `TaskService`<br>`ClientValidator`, `ProjectValidator`, `TaskValidator` |
| **Developer 2: Finance, Security & Concurrency** | • Invoices & Payment Transactions<br>• Remaining Balance Logic (BR-17)<br>• Invoice Status Hierarchy (BR-16)<br>• Atomic Payment Engine & Concurrency (BR-19, NFR-11)<br>• AuditLog Recording (BR-26, BR-29)<br>• Authentication & Invitation Tokens (BR-25)<br>• Central AccessService (BR-27)<br>• Validators: Invoice, Payment | `IInvoiceRepository`<br>`InvoiceRepository`<br>`IInvoiceService` → `InvoiceService`<br>`IAuthService` → `AuthService`<br>`IAccessService` → `AccessService`<br>`InvoiceValidator`<br>`PaymentValidator` |

---

## 5. Detailed Task Specifications

### Task 1 — Concrete Domain Repositories

All repositories inherit from `Repository<T>` and implement their specific interface.

#### 1.1 `ClientRepository` (Developer 1)
* **Location**: `App.BLL/Repositories/ClientRepository.cs`
* **Interface**: `IClientRepository`
* **Required Methods**:
  1. `Task<List<Client>> GetClientsByFreelancerIdAsync(string freelancerId)`:
     * Returns clients where `UserId == freelancerId` and `!IsDeleted`.
     * Use `AsNoTracking()` for read-only listings.
  2. `Task<Client?> GetClientWithProjectsAsync(int clientId, string freelancerId)`:
     * Returns the client matching `Id == clientId` and `UserId == freelancerId` and `!IsDeleted`.
     * Eagerly include `Projects` via `.Include(c => c.Projects)`.

#### 1.2 `ProjectRepository` (Developer 1)
* **Location**: `App.BLL/Repositories/ProjectRepository.cs`
* **Interface**: `IProjectRepository`
* **Required Methods**:
  1. `Task<List<Project>> GetProjectsByFreelancerIdAsync(string freelancerId)`:
     * Returns projects belonging to clients owned by `freelancerId`.
     * Must eagerly load `Client`, `Milestones`, and their `Tasks` (using `.Include(p => p.Milestones).ThenInclude(m => m.Tasks)`).
     * Must eagerly load `Invoices`.
  2. `Task<Project?> GetProjectDetailsAsync(int projectId, string freelancerId)`:
     * Returns project details with `Client`, `Milestones` (ordered by `SortOrder`), `Tasks` (ordered by `SortOrder`), and `Invoices`.
     * Verifies that the owning client belongs to `freelancerId`.
  3. `Task<List<Project>> GetProjectsByClientIdAsync(int clientId)`:
     * Returns projects where `ClientId == clientId` for client portal consumption.

#### 1.3 `InvoiceRepository` (Developer 2)
* **Location**: `App.BLL/Repositories/InvoiceRepository.cs`
* **Interface**: `IInvoiceRepository`
* **Required Methods**:
  1. `Task<List<Invoice>> GetInvoicesByFreelancerIdAsync(string freelancerId)`:
     * Retrieves invoices where `Project.Client.UserId == freelancerId`.
     * Includes `Project` and `Payments`.
  2. `Task<Invoice?> GetInvoiceWithPaymentsAsync(int invoiceId)`:
     * Retrieves the single invoice including `Payments` and the parent `Project`.
  3. `Task<List<Invoice>> GetInvoicesByClientIdAsync(int clientId)`:
     * Retrieves invoices for projects belonging to `clientId` (for client portal).

---

### Task 2 — Pure Calculation Engines & Algorithms

These calculation methods must be implemented as deterministic, testable algorithms.

#### 2.1 Project Progress Calculation (BR-10) — Developer 1
* **Signature**: `int CalculateProgress(Project project)`
* **Business Rules**:
  * Extract all tasks across all milestones in the project: `allTasks = project.Milestones.SelectMany(m => m.Tasks).ToList()`.
  * If `allTasks` is empty: **return 0**.
  * Completed tasks count: `completedCount = allTasks.Count(t => t.Status == TaskItemStatus.Completed)`.
  * Formula: `(completedCount / (double)totalCount) * 100.0`.
  * Rounding: Round to the nearest integer `(int)Math.Round(...)` for display.

#### 2.2 Overdue Task Detection (BR-11) — Developer 1
* **Business Rules**:
  * A task is overdue if and only if:
    1. Its status is NOT `TaskItemStatus.Completed`.
    2. Its `DueDate` is strictly before "Today" (`task.DueDate < _clock.CairoToday`).
  * If `DueDate == _clock.CairoToday`: **The task is NOT overdue.**

#### 2.3 Milestone Status Engine (BR-12) — Developer 1
* **Signature**: `MilestoneStatus CalculateMilestoneStatus(Milestone milestone)`
* **Business Rules** (evaluate in order):
  1. If `milestone.Tasks` is empty OR all tasks have `Status == TaskItemStatus.Pending`:
     * Return `MilestoneStatus.Pending`.
  2. If all tasks have `Status == TaskItemStatus.Completed`:
     * Return `MilestoneStatus.Completed`.
  3. Otherwise:
     * Return `MilestoneStatus.InProgress`.

#### 2.4 Project Health State Machine (Section 8 & BR-31) — Developer 1
* **Signature**: `ProjectHealth CalculateHealth(Project project)`
* **Pre-conditions**:
  * Health is calculated only for projects in `NotStarted`, `InProgress`, or `OnHold`.
  * **BR-31**: If the project has NO tasks (`totalTasks == 0`), health is **`ProjectHealth.Healthy`**.
* **Variables**:
  * $O$ = Number of overdue tasks in the project (evaluated using BR-11).
  * $D$ = Days between `project.Deadline` and `_clock.CairoToday`: `(project.Deadline - _clock.CairoToday).Days`.
    *(A negative value means the project deadline has passed).*
  * $P$ = Exact unrounded progress percentage: `(completedTasks / (double)totalTasks) * 100.0`.
* **State Decision Table** (evaluate in strict priority order):

| Priority | State | Trigger Condition |
| :---: | :--- | :--- |
| **1** | **`ProjectHealth.Critical`** | $O \ge 3$ <br>**OR** ($D \le 3$ and $P < 50.0$) <br>**OR** ($D < 0$ and $P < 100.0$) |
| **2** | **`ProjectHealth.AttentionRequired`** | $O \in \{1, 2\}$ <br>**OR** ($D \le 7$ and $P < 70.0$) |
| **3** | **`ProjectHealth.Healthy`** | Otherwise |

---

#### 2.5 Remaining Balance Calculation (BR-17) — Developer 2
* **Signature**: `decimal CalculateRemainingBalance(Invoice invoice)`
* **Business Rules**:
  * `totalPayments = invoice.Payments.Sum(p => p.Amount)`.
  * `remaining = invoice.Amount - totalPayments`.
  * If `remaining < 0`: clamp to `0m` (balance never drops below zero).
  * Return `remaining`.

#### 2.6 Invoice Status Calculation (BR-16) — Developer 2
* **Signature**: `InvoiceStatus CalculateInvoiceStatus(Invoice invoice)`
* **Business Rules** (evaluate in this exact sequential order; the first matching condition wins):
  1. `CalculateRemainingBalance(invoice) == 0`:
     * Return **`InvoiceStatus.Paid`**.
  2. `invoice.DueDate < _clock.CairoToday`:
     * Return **`InvoiceStatus.Overdue`**.
  3. `invoice.Payments.Any()` (Total payments > 0):
     * Return **`InvoiceStatus.PartiallyPaid`**.
  4. Default:
     * Return **`InvoiceStatus.Unpaid`**.

---

### Task 3 — Core Domain Services (Developer 1)

#### 3.1 `ClientService`
* **Implement**: `IClientService`
* **Inject**: `IClientRepository`, `IAccessService`, `IClock`.
* **Logic Requirements**:
  * `GetClientsAsync`: Filter by `freelancerId` and `!IsDeleted`. If `searchName` is provided, filter case-insensitively.
  * `CreateClientAsync`: Ensure `client.UserId = freelancerId`, `CreatedAtUtc = _clock.UtcNow`, `IsDeleted = false`.
  * `UpdateClientAsync`: Verify ownership via `IAccessService`. Update editable fields (`CompanyName`, `Phone`, `Address`, `Notes`).
  * `SoftDeleteClientAsync`:
    * Verify ownership.
    * Set `client.IsDeleted = true` and `client.DeletedAtUtc = _clock.UtcNow`.
    * Save changes.
    * **BR-05 / BR-26**: When a client is soft-deleted, their linked user account (`Users.ClientId`) is immediately blocked from logging in.

#### 3.2 `ProjectService`
* **Implement**: `IProjectService`
* **Inject**: `IProjectRepository`, `IClientRepository`, `IAccessService`, `IClock`.
* **Logic Requirements**:
  * `CreateProjectAsync`:
    * Verify the chosen `ClientId` belongs to `freelancerId` (BR-04).
    * Enforce `Deadline > StartDate` (BR-01).
    * Initial status must be `ProjectStatus.NotStarted` (FR-12).
    * Initial currency must be from approved list (`EGP`, `USD`, `EUR`).
  * `UpdateProjectStatusAsync`:
    * Status transitions are manual only (BR-07, FR-12).
  * `DeleteProjectAsync`:
    * **BR-06 / FR-13**: Check if the project has any invoices.
    * If invoices exist: **Throw an invalid operation exception or refuse deletion** (status can only be changed to `Cancelled`).
    * If no invoices exist: Delete project (cascades milestones and tasks).

#### 3.3 `TaskService`
* **Implement**: `ITaskService`
* **Inject**: `IRepository<Milestone>`, `IRepository<TaskItem>`, `IProjectRepository`, `IClock`.
* **Logic Requirements**:
  * `AddMilestoneAsync`:
    * Enforce `Milestone.DueDate` is between `Project.StartDate` and `Project.Deadline` inclusive (BR-02).
  * `AddTaskAsync`:
    * Enforce `Task.DueDate <= Milestone.DueDate` (BR-03).
    * Initial status is `TaskItemStatus.Pending`.
  * `UpdateTaskStatusAsync`:
    * Changing to `Completed` is manual only (FR-17). Passing due date does NOT auto-complete.
  * `GetOverdueTasksAsync`:
    * Retrieve all incomplete tasks where `DueDate < _clock.CairoToday` across all active projects of the freelancer.

---

### Task 4 — Financial Transactions, Concurrency & Security (Developer 2)

#### 4.1 `InvoiceService`
* **Implement**: `IInvoiceService`
* **Inject**: `IInvoiceRepository`, `IRepository<Payment>`, `IRepository<AuditLog>`, `AppDbContext`, `IAccessService`, `IClock`.
* **Logic Requirements**:
  * `CreateInvoiceAsync`:
    * Verify project belongs to freelancer.
    * `Invoice.Amount > 0` (BR-14).
    * `Invoice.DueDate >= Invoice.IssueDate` (BR-15).
    * Lock currency to project currency (BR-08).
  * `DeleteInvoiceAsync`:
    * **BR-21**: Check if any payment exists. If `invoice.Payments.Any()`, **deletion is strictly forbidden**.
  * `RecordPaymentAsync` (Atomic Transaction & Concurrency — BR-18, BR-19, NFR-11):
    1. Begin an execution strategy transaction: `using var transaction = await _context.Database.BeginTransactionAsync()`.
    2. Load the invoice fresh from the database including existing payments (do not rely on cached screen values).
    3. Calculate current remaining balance: `balance = CalculateRemainingBalance(invoice)`.
    4. **Validate Amount**:
       * If `amount <= 0` or `amount > balance`: throw validation exception ("Payment amount cannot exceed remaining balance").
    5. **Validate PaymentDate**:
       * Enforce `paymentDate <= _clock.CairoToday` (BR-20: no future payments).
    6. Insert new `Payment`:
       * `Amount = amount`, `Currency = invoice.Currency`, `PaymentDate = paymentDate`, `InvoiceId = invoiceId`, `CreatedAtUtc = _clock.UtcNow`.
    7. Write `AuditLog` entry inside the **same transaction** (BR-19, BR-29):
       * `EntityName = "Payment"`, `Action = "Create"`, `UserId = freelancerId`, `CreatedAtUtc = _clock.UtcNow`.
    8. Try `await _context.SaveChangesAsync()`.
       * **Catch `DbUpdateConcurrencyException`**: If another transaction updated the invoice row concurrently, roll back and throw a specific concurrency exception with message: *"The invoice was updated concurrently, please try again."*
    9. Commit transaction: `await transaction.CommitAsync()`.

#### 4.2 `AuthService`
* **Implement**: `IAuthService`
* **Inject**: `UserManager<User>`, `IRepository<Invitation>`, `IClock`, `IConfiguration`.
* **Logic Requirements**:
  * `RegisterFreelancerAsync`:
    * Self-registration allowed only for `UserRole.Freelancer` (FR-01).
    * Normalizes email to lowercase and trimmed (BR-23).
  * `GenerateClientInvitationLinkAsync` (FR-05, BR-25):
    * Generate a cryptographically secure random token (at least 256 bits, e.g. `RandomNumberGenerator.GetBytes(32)` converted to Base64/Hex).
    * Compute `TokenHash = SHA256(rawToken)`.
    * Set `ExpiresAtUtc = _clock.UtcNow.AddDays(7)` (7-day validity).
    * If a previous unused invitation exists for this client, revoke it (`RevokedAtUtc = _clock.UtcNow`).
    * Save `Invitation` record with `TokenHash`.
    * Return full invitation URL containing the plain token in query string (the plain token is never stored in DB).
  * `RegisterClientByInvitationTokenAsync`:
    * Hash incoming token with SHA256 and look up `Invitation`.
    * Verify invitation exists, `UsedAtUtc == null`, `RevokedAtUtc == null`, and `ExpiresAtUtc > _clock.UtcNow`.
    * Create `User` with `Role = UserRole.Client`, `ClientId = invitation.ClientId`.
    * Mark invitation as used: `UsedAtUtc = _clock.UtcNow`.

#### 4.3 `AccessService`
* **Implement**: `IAccessService`
* **Inject**: `AppDbContext`.
* **Logic Requirements (BR-27)**:
  * Central authority for checking permissions:
    * `CanFreelancerAccessClientAsync(freelancerId, clientId)`: Returns true if client exists, `UserId == freelancerId`, and `!IsDeleted`.
    * `CanFreelancerAccessProjectAsync(freelancerId, projectId)`: Returns true if project exists and `project.Client.UserId == freelancerId` and `!project.Client.IsDeleted`.
    * `CanClientAccessProjectAsync(clientUserId, projectId)`: Returns true if user has `ClientId != null`, client is not deleted, and `project.ClientId == user.ClientId`.

---

### Task 5 — FluentValidation Rules

Create validator classes inside `App.BLL/Validations/` inheriting from `AbstractValidator<T>`.

#### 5.1 `ClientValidator` (Developer 1)
| Property | Rule | Error Message |
| :--- | :--- | :--- |
| `CompanyName` | `NotEmpty()`, `MaximumLength(150)` | "Company name is required and cannot exceed 150 characters." |
| `Phone` | `NotEmpty()`, `Matches(@"^[0-9+\s-]{8,20}$")` | "Please enter a valid phone number." |
| `Address` | `MaximumLength(250)` | "Address cannot exceed 250 characters." |

#### 5.2 `ProjectValidator` (Developer 1)
| Property | Rule | Error Message |
| :--- | :--- | :--- |
| `Name` | `NotEmpty()`, `MaximumLength(150)` | "Project name is required." |
| `ClientId` | `GreaterThan(0)` | "Please select a valid client." |
| `Budget` | `GreaterThan(0)` | "Budget must be greater than zero." |
| `Currency` | `Must(c => c in ["EGP", "USD", "EUR"])` | "Currency must be EGP, USD, or EUR." |
| `StartDate` | `NotEmpty()` | "Start date is required." |
| `Deadline` | `GreaterThan(p => p.StartDate)` | "Deadline must be after start date." |

#### 5.3 `TaskValidator` (Developer 1)
| Property | Rule | Error Message |
| :--- | :--- | :--- |
| `Title` | `NotEmpty()`, `MaximumLength(200)` | "Task title is required." |
| `Priority` | `IsInEnum()` | "Please select a valid priority." |
| `DueDate` | `NotEmpty()` | "Due date is required." |

#### 5.4 `InvoiceValidator` (Developer 2)
| Property | Rule | Error Message |
| :--- | :--- | :--- |
| `ProjectId` | `GreaterThan(0)` | "Please link the invoice to a project." |
| `Amount` | `GreaterThan(0)` | "Invoice amount must be greater than zero." |
| `IssueDate` | `NotEmpty()` | "Issue date is required." |
| `DueDate` | `GreaterThanOrEqualTo(i => i.IssueDate)` | "Due date must be on or after issue date." |

#### 5.5 `PaymentValidator` (Developer 2)
| Property | Rule | Error Message |
| :--- | :--- | :--- |
| `Amount` | `GreaterThan(0)` | "Payment amount must be greater than zero." |
| `PaymentDate` | `NotEmpty()` | "Payment date is required." |

---

### Task 6 — Dependency Injection Registration

Both developers register their completed services and repositories in:
`App.BLL/Extensions/BllServiceExtensions.cs`.

Ensure the extension method adheres to:
```csharp
public static IServiceCollection AddBLL(this IServiceCollection services)
{
    // Clock
    services.AddSingleton<IClock, SystemClock>();

    // Repositories
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<IClientRepository, ClientRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();
    services.AddScoped<IInvoiceRepository, InvoiceRepository>();

    // Domain Services
    services.AddScoped<IAccessService, AccessService>();
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IClientService, ClientService>();
    services.AddScoped<IProjectService, ProjectService>();
    services.AddScoped<ITaskService, TaskService>();
    services.AddScoped<IInvoiceService, InvoiceService>();

    // FluentValidation Assembly Scan
    services.AddValidatorsFromAssembly(typeof(BllServiceExtensions).Assembly);

    return services;
}
```

---

### Task 7 — Unit Testing Suite (xUnit)

Every pure calculation function must be covered by comprehensive xUnit tests (NFR-13). Create test classes verifying:

1. **Progress Calculation (`CalculateProgress`)**:
   * Project with 0 tasks → returns `0%`.
   * Project with 3 completed out of 4 tasks → returns `75%`.
   * Project with 1 completed out of 3 tasks → returns `33%` (rounded).
2. **Overdue Task Detection (`GetOverdueTasksAsync`)**:
   * Task with `DueDate = Yesterday` and status `Pending` → detected as Overdue.
   * Task with `DueDate = Yesterday` and status `Completed` → NOT overdue.
   * Task with `DueDate = Today` (Cairo Time) and status `Pending` → NOT overdue.
3. **Milestone Status (`CalculateMilestoneStatus`)**:
   * Milestone with no tasks → `Pending`.
   * Milestone with all tasks completed → `Completed`.
   * Milestone with 1 completed, 1 pending → `InProgress`.
4. **Invoice Status (`CalculateInvoiceStatus`)**:
   * Amount 1000, Paid 1000 → `Paid`.
   * Amount 1000, Paid 400, DueDate in future → `PartiallyPaid`.
   * Amount 1000, Paid 0, DueDate in past → `Overdue`.
   * Amount 1000, Paid 0, DueDate in future → `Unpaid`.
5. **Project Health (`CalculateHealth`)**:
   * No tasks → `Healthy` (BR-31).
   * 3 overdue tasks → `Critical`.
   * Deadline in 2 days, Progress 40% → `Critical`.
   * 1 overdue task, Deadline in 20 days → `AttentionRequired`.
   * 0 overdue tasks, Deadline in 20 days, Progress 80% → `Healthy`.

---

## 📋 Evaluation Checklist

Before submitting the BLL layer to the Team Lead, verify every requirement:

- [ ] **Architecture**: `App.BLL` references `App.DAL` only; it contains no MVC dependencies.
- [ ] **Clock Rule**: Not a single instance of `DateTime.Now` exists in `App.BLL`; all dates resolve through `IClock`.
- [ ] **Data Types**: All financial fields and parameters are typed as `decimal(18,2)`.
- [ ] **Generic & Custom Repositories**: `ClientRepository`, `ProjectRepository`, and `InvoiceRepository` are implemented and use `AsNoTracking()` where appropriate.
- [ ] **Soft Delete**: Deleting a client sets `IsDeleted = true` and blocks portal access; no hard delete is performed.
- [ ] **Task & Milestone Hierarchy**: Date cascade rules (BR-01, BR-02, BR-03) are validated upon creation.
- [ ] **Pure Engines**: Progress (BR-10), Overdue (BR-11), Milestone (BR-12), and Health (Section 8) functions pass all edge-case tests.
- [ ] **Financial Integrity**: Payment recording executes inside a database transaction, checks remaining balance on the DB level, and records an `AuditLog` entry.
- [ ] **Concurrency**: Concurrency conflicts on invoices are caught via `DbUpdateConcurrencyException` and re-thrown with a clear user message.
- [ ] **Tokens & Security**: Invitation tokens are 256-bit secure strings, stored only as SHA256 hashes, and expire after 7 days.
- [ ] **Central Authorization**: Every operation verifies ownership through `AccessService`.
- [ ] **Validation**: All 5 FluentValidation validators are registered and validate required rules and regexes.
- [ ] **Unit Tests**: All calculation unit tests run with 100% pass rate.
