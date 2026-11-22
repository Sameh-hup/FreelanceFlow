# ASP.NET Core 3-Tier Architecture — Presentation Layer (MVC) Assignment

## Project: FreelanceFlow — Freelancer & Client Management System 🖥️

---

## 1. Overview & Architectural Role

In FreelanceFlow, the **Presentation Layer (`App.Mvc`)** provides the user experience for three distinct actors: **Freelancers**, **Clients**, and **Admins**. It is responsible for rendering Razor views, capturing user input, triggering client-side validation, passing requests to the Business Logic Layer (`App.BLL`), and communicating results via toast notifications and intuitive UI components.

As the MVC front-end engineering team (2 developers), you are responsible for:
1. **Strict Layering**: The MVC layer **must never reference `AppDbContext` or Entity Framework Core directly**. All data access and business decisions flow through BLL services.
2. **ViewModel Encapsulation**: Form inputs and complex composite views must use dedicated ViewModels with DataAnnotations for client-side feedback.
3. **Reusable Form Partials**: Shared form markup (such as Create and Edit forms) must be extracted into partial views to eliminate code duplication.
4. **Interactive UI & Empty States**: Every listing must handle the empty state gracefully with informative Bootstrap alerts and call-to-action buttons (never empty tables).
5. **Toast Notifications & Alerts**: User feedback must be driven by `TempData` using the toast notification system.
6. **Role-Based UI Rendering**: The navigation bar and view actions must adapt dynamically to the authenticated user's role (`Freelancer`, `Client`, or `Admin`).

> **Team Notice**:
> The Team Lead owns `App.DAL`, and the BLL team is building the services in `App.BLL`. Your controllers will inject interfaces from `App.BLL.Interfaces.Services` (e.g. `IClientService`, `IProjectService`, `IInvoiceService`, `IAuthService`, `IAccessService`).

---

## 2. Architecture & MVC Project Structure

Your project is `App.Mvc` (ASP.NET Core Web App MVC - .NET 10.0), referencing `App.BLL` and `App.DAL`.

```text
App.Mvc
│
├── 📁 Controllers
│   ├── AccountController.cs                (Login, Register, Logout, Invitations)
│   ├── HomeController.cs                   (Freelancer Dashboard & General Errors)
│   ├── ClientsController.cs                (Client CRUD & Invitation Modal)
│   ├── ProjectsController.cs               (Project Workspace, Milestones & Health)
│   ├── TasksController.cs                  (Task status toggling & management)
│   ├── InvoicesController.cs               (Invoices & Payment Recording Modal)
│   ├── AdminController.cs                  (User activation & password resets)
│   └── PortalController.cs                 (Client Read-Only Portal)
│
├── 📁 Models (ViewModels)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ChangePasswordViewModel.cs
│   ├── InvitationRegisterViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ClientViewModel.cs
│   ├── ProjectViewModel.cs
│   ├── TaskViewModel.cs
│   ├── InvoiceViewModel.cs
│   ├── RecordPaymentViewModel.cs
│   └── TosterViewModel.cs
│
├── 📁 Views
│   ├── 📁 Account
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── ChangePassword.cshtml
│   │   ├── RegisterByInvitation.cshtml
│   │   └── AccessDenied.cshtml
│   ├── 📁 Home
│   │   ├── Index.cshtml                    (Freelancer Dashboard with 4 cards)
│   │   └── Privacy.cshtml
│   ├── 📁 Clients
│   │   ├── Index.cshtml                    (Clients grid + Invite modal)
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   ├── 📁 Projects
│   │   ├── Index.cshtml                    (Cards/Grid with Health Badges & Progress)
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Details.cshtml                  (Full workspace: Milestones & Tasks tree)
│   ├── 📁 Invoices
│   │   ├── Index.cshtml                    (Status filter, Invoices table)
│   │   ├── Create.cshtml
│   │   └── Details.cshtml                  (Invoice ledger + Record Payment modal)
│   ├── 📁 Admin
│   │   └── Index.cshtml                    (User accounts list & status toggle)
│   ├── 📁 Portal
│   │   └── Index.cshtml                    (Client read-only project & invoice tracking)
│   └── 📁 Shared
│       ├── _Layout.cshtml                  (Role-aware navigation & theme)
│       ├── _Alerts.cshtml                  (TempData success/error banners)
│       ├── _TosterPartial.cshtml           (Toast notification component)
│       ├── _ValidationScriptsPartial.cshtml
│       ├── _ClientFormFieldsPartial.cshtml
│       ├── _ProjectFormFieldsPartial.cshtml
│       └── _InvoiceFormFieldsPartial.cshtml
│
└── 📁 wwwroot
    ├── 📁 css
    ├── 📁 js
    └── 📁 lib (Bootstrap 5, Bootstrap Icons, jQuery Validation)
```

---

## 3. General Rules & Engineering Guidelines

1. **Server Authority & AntiForgery**:
   * Every `[HttpPost]` action must be decorated with `[ValidateAntiForgeryToken]`.
   * Form validation must run both on client-side (unobtrusive validation scripts) and server-side (`ModelState.IsValid` and BLL FluentValidation).
2. **Access Control & Attribute Placement**:
   * Management controllers (`ClientsController`, `ProjectsController`, `InvoicesController`) must be decorated with `[Authorize(Roles = "Freelancer")]`.
   * `AdminController` must be decorated with `[Authorize(Roles = "Admin")]`.
   * `PortalController` must be decorated with `[Authorize(Roles = "Client")]`.
   * Public actions (`AccountController.Login`, `AccountController.Register`, `RegisterByInvitation`) must be marked `[AllowAnonymous]`.
3. **Empty States Requirement (UI/UX)**:
   * When a list (clients, projects, invoices, tasks) has 0 records, **never render an empty table header**.
   * Render a styled Bootstrap card or alert with an icon, an explanatory message, and a primary button linking to the Create action.
4. **Toast Notifications Protocol**:
   * After any successful state modification (Create, Update, Delete, Record Payment), write a serialized `TosterViewModel` to `TempData["ToastMessage"]` and redirect (`PRG: Post-Redirect-Get` pattern).
   * In case of validation or business rule failures, reload dropdowns and return the submitted view with `ModelState` errors.
5. **No Financial Data in Admin Views (BR-28)**:
   * The Admin views must never display invoices, amounts, payments, or client financial metrics.

---

## 4. Team Division of Responsibilities

To enable seamless parallel delivery, the MVC work is divided between two front-end developers:

| Role | Assigned Controllers, Views & Modules | Key Deliverables |
| :--- | :--- | :--- |
| **Developer 1: Security, Shell, Portals & Dashboard** | • Dynamic Layout & Role-Based Navigation<br>• Account Management & Authentication<br>• Client Invitation Registration Flow (FR-05, BR-25)<br>• Admin Panel & User Status Toggle (FR-03, BR-28)<br>• Client Read-Only Portal (FR-27, FR-28)<br>• Freelancer 4-Metric Dashboard (FR-29)<br>• Toast System Integration & ViewModels | `AccountController` & all Account views<br>`HomeController` & Dashboard view<br>`AdminController` & Admin view<br>`PortalController` & Portal view<br>`_Layout.cshtml`<br>`LoginViewModel`, `RegisterViewModel`, `ChangePasswordViewModel`, `InvitationRegisterViewModel`, `DashboardViewModel` |
| **Developer 2: Core Workflows (Clients, Projects, Tasks & Invoices)** | • Reusable Form Partials (`_ClientFormFieldsPartial`, etc.)<br>• Clients Management & Invitation Link Modal<br>• Projects Management with Health Badges & Progress<br>• Project Details Workspace with Milestones & Tasks Tree<br>• Task Status AJAX / Form Toggling<br>• Invoices Ledger & Details<br>• Record Payment Modal & Concurrency Feedback | `ClientsController` & all Client views<br>`ProjectsController` & all Project views<br>`TasksController` & `MilestonesController`<br>`InvoicesController` & all Invoice views<br>`ClientViewModel`, `ProjectViewModel`, `TaskViewModel`, `InvoiceViewModel`, `RecordPaymentViewModel`<br>Shared Form Partials |

---

## 5. Detailed Task Specifications

### Task 1 — Authentication, Portals & Dashboard (Developer 1)

#### 1.1 `AccountController`
* **Inject**: `IAuthService`, `SignInManager<User>`, `UserManager<User>`.
* **Actions to Implement**:
  * `Login(string? returnUrl)` [HttpGet, AllowAnonymous]: Returns `Login.cshtml` with `ReturnUrl`.
  * `Login(LoginViewModel model)` [HttpPost, AllowAnonymous]:
    * Validate `ModelState`.
    * Authenticate using `SignInManager.PasswordSignInAsync`.
    * If `MustChangePassword` is true on the user record, redirect to `ChangePassword`.
    * Redirect to `returnUrl` (if local) or appropriate dashboard based on role:
      * `Admin` → `Admin/Index`
      * `Client` → `Portal/Index`
      * `Freelancer` → `Home/Index`
    * On failure: Add generic error *"Invalid email or password"* (never reveal which was wrong).
  * `Register()` [HttpGet, AllowAnonymous]: Public registration for Freelancers only (FR-01).
  * `Register(RegisterViewModel model)` [HttpPost, AllowAnonymous]:
    * Call `IAuthService.RegisterFreelancerAsync`.
    * Add success toast and redirect to `Login`.
  * `RegisterByInvitation(string token)` [HttpGet, AllowAnonymous]:
    * Display registration form with client's pre-filled invitation details (FR-05, BR-25).
  * `RegisterByInvitation(InvitationRegisterViewModel model)` [HttpPost, AllowAnonymous]:
    * Call `IAuthService.RegisterClientByInvitationTokenAsync`.
    * Redirect to `Login` with welcome message.
  * `ChangePassword()` [HttpGet, Authorize]: Mandatory change password screen (FR-04).
  * `ChangePassword(ChangePasswordViewModel model)` [HttpPost, Authorize]:
    * Updates password and clears `MustChangePassword` flag.
  * `Logout()` [HttpPost, Authorize]:
    * Calls `SignInManager.SignOutAsync()` and redirects to `Home/Index`.
  * `AccessDenied()` [HttpGet, AllowAnonymous]: Renders friendly 403 error page.

#### 1.2 `HomeController` & Freelancer Dashboard (FR-29)
* **Inject**: `IClientService`, `IProjectService`, `ITaskService`, `IInvoiceService`, `UserManager<User>`.
* **Action**: `Index()` [HttpGet, Authorize(Roles = "Freelancer")]:
  * Construct `DashboardViewModel` with exactly 4 statistics:
    1. **Total Clients Count**: Count of active clients owned by the freelancer.
    2. **Active Projects Count**: Projects in `InProgress` or `NotStarted` status.
    3. **Overdue Tasks Count**: Result from `ITaskService.GetOverdueTasksAsync`.
    4. **Unpaid Invoices Count**: Count of invoices with status `Unpaid` or `PartiallyPaid` or `Overdue`.
  * Render 4 colorful Bootstrap stat cards (`bg-primary`, `bg-success`, `bg-danger`, `bg-warning`) with icons.

#### 1.3 `AdminController` (FR-03, BR-28)
* **Inject**: `UserManager<User>`.
* **Authorize**: `[Authorize(Roles = "Admin")]`.
* **Action**: `Index()`:
  * Lists all users in the system (Name, Email, Role, IsActive, CreatedAt).
  * Strictly **no financial data, project budgets, or invoices** (BR-28).
* **Action**: `ToggleUserStatus(string userId)` [HttpPost]:
  * Toggle `IsActive` property. If user is deactivated, their login is immediately blocked (BR-26).
  * Toast message: *"User account status updated."*

#### 1.4 `PortalController` (FR-27, FR-28)
* **Inject**: `IProjectService`, `IInvoiceService`, `UserManager<User>`.
* **Authorize**: `[Authorize(Roles = "Client")]`.
* **Action**: `Index()`:
  * Retrieve current logged-in client's `ClientId` from `User.Claims`.
  * Load only projects belonging to this client:
    * Displays project name, deadline, progress bar, and milestones list.
    * Displays invoices table (Amount, DueDate, Status badge: Paid, Overdue, etc.).
  * **Strictly Read-Only**: No edit buttons, no delete buttons, and no payment creation controls.

#### 1.5 Shared Shell & Navigation (`_Layout.cshtml`)
* Implement dynamic navbar:
  * **Anonymous Users**: Brand logo, Login button.
  * **Freelancer**: Brand logo, Dashboard, Clients, Projects, Invoices, User profile dropdown with Logout.
  * **Client**: Brand logo, My Projects & Invoices (`Portal`), User profile dropdown with Logout.
  * **Admin**: Brand logo, User Management (`Admin`), Change Password, Logout.

---

### Task 2 — Core Business Workflows (Developer 2)

#### 2.1 `ClientsController`
* **Inject**: `IClientService`, `IAuthService`, `UserManager<User>`.
* **Authorize**: `[Authorize(Roles = "Freelancer")]`.
* **Actions to Implement**:
  * `Index(string? search)`:
    * Call `_clientService.GetClientsAsync(freelancerId, search)`.
    * Render clients table/cards with search input. Include an *"Invite Client"* button that opens a Bootstrap modal.
    * Handle empty state with an alert: *"No clients found. Click 'Add Client' to create your first client."*
  * `Create()` [HttpGet]: Returns `Create.cshtml` with empty `ClientViewModel`.
  * `Create(ClientViewModel model)` [HttpPost]:
    * Validate `ModelState`.
    * Call `_clientService.CreateClientAsync(model.ToEntity(), freelancerId)`.
    * Toast: *"Client '{CompanyName}' created successfully."* Redirect to `Index`.
  * `Edit(int id)` [HttpGet]: Load client, map to `ClientViewModel`, render `Edit.cshtml`.
  * `Edit(int id, ClientViewModel model)` [HttpPost]:
    * Call `_clientService.UpdateClientAsync(model.ToEntity(), freelancerId)`.
    * Toast: *"Client updated successfully."* Redirect to `Index`.
  * `Details(int id)`: Displays client information and list of associated projects with progress bars.
  * `GenerateInvitation(int clientId, string clientEmail)` [HttpPost]:
    * Call `_authService.GenerateClientInvitationLinkAsync`.
    * Return JSON or reload with modal displaying the generated URL and a **"Copy Link"** button (FR-05).
  * `Delete(int id)` [HttpPost]:
    * Call `_clientService.SoftDeleteClientAsync(id, freelancerId)`.
    * Toast: *"Client moved to trash."* Redirect to `Index`.

#### 2.2 `ProjectsController`
* **Inject**: `IProjectService`, `IClientService`, `ITaskService`, `UserManager<User>`.
* **Authorize**: `[Authorize(Roles = "Freelancer")]`.
* **Actions to Implement**:
  * `Index(string? search)`:
    * Fetch projects with calculated progress and health.
    * Render table/cards showing:
      * Project Name, Client Name, Budget & Currency.
      * **Progress Bar**: `width: @project.Progress%` with percentage text.
      * **Health Badge**:
        * `Healthy` → `<span class="badge bg-success">Healthy</span>`
        * `AttentionRequired` → `<span class="badge bg-warning text-dark">Attention Required</span>`
        * `Critical` → `<span class="badge bg-danger">Critical</span>`
      * Action buttons: View Workspace, Edit, Delete.
  * `Create()` [HttpGet]:
    * Populate client dropdown (`ViewBag.Clients`) from `IClientService.GetClientsAsync`.
    * Initial status: `NotStarted` (FR-12). Supported currencies: `EGP`, `USD`, `EUR` (FR-11).
  * `Create(ProjectViewModel model)` [HttpPost]:
    * Verify `Deadline > StartDate`.
    * Call `_projectService.CreateProjectAsync`.
    * Toast: *"Project '{Name}' initialized."* Redirect to `Index`.
  * `Details(int id)` — **The Project Workspace**:
    * Displays Project Header: Budget, Health Badge, Overall Progress %, Deadline countdown.
    * Milestones & Tasks Tree:
      * Accordion list of milestones (Name, DueDate, Milestone Status badge).
      * Nested list of tasks under each milestone:
        * Task Title, Due Date, Priority badge (Low, Med, High).
        * Task Status Dropdown / Toggle: `Pending`, `InProgress`, `Completed`.
        * Overdue Indicator: Red warning icon `<i class="bi bi-clock-history text-danger"></i>` if task is overdue.
      * Modals/Forms to "Add Milestone" and "Add Task".
    * Invoices tab: Invoices linked to this project and their payment statuses.
  * `Delete(int id)` [HttpPost]:
    * Call `_projectService.DeleteProjectAsync`.
    * If project has invoices: Show error toast *"Cannot delete a project with invoices. Status changed to Cancelled."* (BR-06, FR-13).

#### 2.3 `InvoicesController` & Payment Recording
* **Inject**: `IInvoiceService`, `IProjectService`, `UserManager<User>`.
* **Authorize**: `[Authorize(Roles = "Freelancer")]`.
* **Actions to Implement**:
  * `Index(string? search)`:
    * Display invoices with columns: Invoice #, Project, Client, Amount, Due Date, Balance, Status Badge.
    * Status Badge mapping (BR-16):
      * `Paid` → `bg-success`
      * `PartiallyPaid` → `bg-info`
      * `Unpaid` → `bg-secondary`
      * `Overdue` → `bg-danger`
  * `Create(int? projectId)` [HttpGet]:
    * Populate project dropdown. Lock currency to selected project's currency.
  * `Create(InvoiceViewModel model)` [HttpPost]:
    * Validate `DueDate >= IssueDate` and `Amount > 0`.
    * Call `_invoiceService.CreateInvoiceAsync`.
    * Toast: *"Invoice created successfully."* Redirect to `Details`.
  * `Details(int id)`:
    * Displays invoice metadata, total amount, total paid, and remaining balance.
    * Payments history table: Payment Date, Amount, Notes.
    * **Record Payment Modal** (opens via button):
      * Form with `Amount`, `PaymentDate` (defaulting to today), `Notes`.
  * `RecordPayment(RecordPaymentViewModel model)` [HttpPost]:
    * Call `_invoiceService.RecordPaymentAsync`.
    * Handle **Concurrency**: If `DbUpdateConcurrencyException` occurs, show error toast: *"The invoice was updated concurrently, please try again."* (BR-19, NFR-11).
    * If amount exceeds balance: Show error toast *"Payment amount cannot exceed remaining balance."* (BR-18).
    * On success: Toast *"Payment of {Amount} recorded successfully."* Redirect to `Details`.
  * `Delete(int id)` [HttpPost]:
    * Call `_invoiceService.DeleteInvoiceAsync`.
    * If invoice has payments: Display error toast *"Invoices with recorded payments cannot be deleted."* (BR-21).

---

### Task 3 — ViewModels & Data Annotations Specification

Define strongly-typed ViewModels in `App.Mvc/Models/`:

#### 3.1 `LoginViewModel` (Developer 1)
| Property | Type | DataAnnotations | UI Element |
| :--- | :--- | :--- | :--- |
| `Email` | `string` | `[Required]`, `[EmailAddress]` | `<input type="email" />` |
| `Password` | `string` | `[Required]`, `[DataType(DataType.Password)]` | `<input type="password" />` |
| `RememberMe` | `bool` | — | `<input type="checkbox" />` |
| `ReturnUrl` | `string?` | — | `<input type="hidden" />` |

#### 3.2 `RegisterViewModel` (Developer 1)
| Property | Type | DataAnnotations | UI Element |
| :--- | :--- | :--- | :--- |
| `Name` | `string` | `[Required]`, `[StringLength(100)]` | Text input |
| `Email` | `string` | `[Required]`, `[EmailAddress]` | Email input |
| `Password` | `string` | `[Required]`, `[MinLength(8)]`, `[DataType(DataType.Password)]` | Password input |
| `ConfirmPassword`| `string` | `[Compare("Password", ErrorMessage = "Passwords do not match")]` | Password input |

#### 3.3 `ClientViewModel` (Developer 2)
| Property | Type | DataAnnotations | UI Element |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | — | Hidden input (on Edit) |
| `CompanyName` | `string` | `[Required]`, `[StringLength(150)]` | Text input |
| `Phone` | `string` | `[Required]`, `[Phone]` | Phone input |
| `Address` | `string?` | `[StringLength(250)]` | Text input |
| `Notes` | `string?` | — | Textarea (3 rows) |

#### 3.4 `ProjectViewModel` (Developer 2)
| Property | Type | DataAnnotations | UI Element |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | — | Hidden input (on Edit) |
| `ClientId` | `int` | `[Required]`, `[Range(1, int.MaxValue)]` | `<select>` dropdown |
| `Name` | `string` | `[Required]`, `[StringLength(150)]` | Text input |
| `Description` | `string?` | — | Textarea |
| `Budget` | `decimal` | `[Required]`, `[Range(0.01, 10000000.0)]` | Number input (`step="0.01"`) |
| `Currency` | `string` | `[Required]` | `<select>` (`EGP`, `USD`, `EUR`) |
| `StartDate` | `DateOnly`| `[Required]` | Date input (`type="date"`) |
| `Deadline` | `DateOnly`| `[Required]` | Date input (`type="date"`) |

#### 3.5 `RecordPaymentViewModel` (Developer 2)
| Property | Type | DataAnnotations | UI Element |
| :--- | :--- | :--- | :--- |
| `InvoiceId` | `int` | `[Required]` | Hidden input |
| `Amount` | `decimal` | `[Required]`, `[Range(0.01, 1000000.0)]` | Number input (`step="0.01"`) |
| `PaymentDate`| `DateOnly`| `[Required]` | Date input (`type="date"`) |
| `Notes` | `string?` | `[StringLength(500)]` | Text input |

---

### Task 4 — Reusable Form Partials (Developer 2)

Create partial views in `App.Mvc/Views/Shared/`:
1. `_ClientFormFieldsPartial.cshtml` (`@model ClientViewModel`):
   * Includes fields for `CompanyName`, `Phone`, `Address`, `Notes`.
   * Include `<span asp-validation-for="..." class="text-danger"></span>`.
2. `_ProjectFormFieldsPartial.cshtml` (`@model ProjectViewModel`):
   * Includes fields for `ClientId`, `Name`, `Budget`, `Currency`, `StartDate`, `Deadline`.
3. `_InvoiceFormFieldsPartial.cshtml` (`@model InvoiceViewModel`):
   * Includes fields for `ProjectId`, `Amount`, `IssueDate`, `DueDate`.

Usage Pattern in `Create.cshtml` and `Edit.cshtml`:
```html
<form asp-action="Create" method="post">
    @Html.AntiForgeryToken()
    <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
    
    <partial name="_ClientFormFieldsPartial" model="Model" />
    
    <div class="mt-4">
        <button type="submit" class="btn btn-primary">Save Client</button>
        <a asp-action="Index" class="btn btn-outline-secondary">Cancel</a>
    </div>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

---

### Task 5 — Toast Notification System Integration

Use `TosterViewModel` to trigger alerts on the subsequent request:

```csharp
TempData["ToastMessage"] = JsonSerializer.Serialize(new TosterViewModel
{
    Type = TosterType.Success,
    Content = $"Client '{model.CompanyName}' has been saved successfully."
});
```

#### Toast Messages Specification:
| Triggering Action | Toast Type | Message Template |
| :--- | :--- | :--- |
| Create Client / Project / Invoice | `Success` | `"{Entity} '{Name}' was created successfully."` |
| Edit Client / Project | `Info` | `"{Entity} '{Name}' was updated."` |
| Soft Delete Client | `Warning` | `"Client '{Name}' was moved to trash."` |
| Delete Project with Invoices | `Error` | `"Cannot delete a project with invoices. Status changed to Cancelled."` |
| Record Payment Success | `Success` | `"Payment of {Amount} recorded successfully."` |
| Overpayment Attempt | `Error` | `"Payment amount cannot exceed remaining balance."` |
| Concurrency Conflict | `Error` | `"The invoice was updated concurrently, please try again."` |
| Account Locked / Deactivated | `Error` | `"This account has been deactivated."` |

---

## 📋 Evaluation Checklist

Before presenting the presentation layer to the Team Lead, verify every requirement:

- [ ] **Layering**: `App.Mvc` does not import or instantiate `AppDbContext`; all data operations call `App.BLL` interfaces.
- [ ] **Security**: Every form post uses `@Html.AntiForgeryToken()` and actions carry `[ValidateAntiForgeryToken]`.
- [ ] **Role Protection**:
  * `[Authorize(Roles = "Freelancer")]` guards `Clients`, `Projects`, `Invoices`.
  * `[Authorize(Roles = "Admin")]` guards `AdminController`.
  * `[Authorize(Roles = "Client")]` guards `PortalController`.
- [ ] **Empty States**: If no clients, projects, or invoices exist, a friendly Bootstrap alert is displayed with a call-to-action button (no empty table headers).
- [ ] **Dashboard (FR-29)**: The Freelancer dashboard renders exactly 4 summary metric cards.
- [ ] **Client Portal (FR-27, FR-28)**: Clients see only their projects and invoices in a read-only layout with no modification controls.
- [ ] **Admin Separation (BR-28)**: The Admin panel lists accounts and activation toggles with zero financial data.
- [ ] **Client Invitation Flow (FR-05)**: Freelancer can generate an invitation link, copy it, and a client can register via that link.
- [ ] **Health & Progress Visualization**: Projects list and details show an active progress bar and colored health badges (`Healthy`, `AttentionRequired`, `Critical`).
- [ ] **Financial UI**: The Invoice Details page features a modal to record payments, displays remaining balance, and handles concurrency errors gracefully.
- [ ] **Validation Feedback**: Client-side validation (`_ValidationScriptsPartial`) and server-side FluentValidation errors appear cleanly beneath form inputs.
- [ ] **Toast Lifecycle**: Toast notifications appear after redirect, display appropriate colors/icons, and disappear on the subsequent request.
