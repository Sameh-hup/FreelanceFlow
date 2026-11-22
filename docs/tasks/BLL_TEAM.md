# 📌 مهام فريق الـ Business Logic Layer (BLL)

مرحباً يا أبطال! البنية التحتية والـ Interfaces جاهزة بالكامل عشان تبدأوا كتابة الكود مباشرة دون أي حيرة.

---

## 📂 الملفات المطلوب كتابتها وإكمالها:

### 1. في مجلد `App.BLL/Repositories`:
- إكمال الـ Repositories المخصصة التي ترث من `Repository<T>`:
  - `ClientRepository.cs` (تطبيق `IClientRepository`)
  - `ProjectRepository.cs` (تطبيق `IProjectRepository`)
  - `InvoiceRepository.cs` (تطبيق `IInvoiceRepository`)

---

### 2. في مجلد `App.BLL/Services`:
- إنشاء الـ Implementations للـ Services الموجودة في `Interfaces/Services`:
  - **`AuthService.cs`**:
    - تسجيل دخول ومقارنة الـ Hash بـ `BCrypt.Net`.
    - توليد توكن الدعوة للعميل وتخزين الـ SHA256 Hash تبعه.
  - **`ClientService.cs`**:
    - الـ CRUD مع التأكد من الـ Soft Delete (`IsDeleted = true`).
  - **`ProjectService.cs`**:
    - إدارة المشاريع.
    - حساب نسبة الـ Progress: `(المهام المكتملة ÷ الإجمالي) * 100`.
    - حساب الـ Health: (`Critical`, `AttentionRequired`, `Healthy`) طبقاً لقواعد القسم 8.
  - **`TaskService.cs`**:
    - إضافة المراحل والمهام، وفحص الـ Overdue Tasks مقارنة بـ `IClock.UtcToday`.
  - **`InvoiceService.cs`**:
    - إنشاء الفاتورة، تسجيل الدفعة داخل Transaction والتأكد من عدم تجاوز الرصيد المتبقي (منع Overpayment) والتعامل مع `DbUpdateConcurrencyException`.
  - **`AccessService.cs`**:
    - فحص الملكية المركزي قبل أي عملية.

---

### 3. التسجيل في الـ Dependency Injection:
- تسجيل أي Service أو Repository جديد داخل:
  `App.BLL/Extensions/BllServiceExtensions.cs`.

---

## ⚠️ قواعد هامة:
1. أي تعامل مع التواريخ يتم عبر حقن `IClock` (استخدم `_clock.UtcNow` أو `_clock.UtcToday`). ممنوع استخدام `DateTime.Now`.
2. لا تعدل في `App.DAL/Entities` أو الـ Migrations مباشرة؛ كل تعديل في الـ Schema يمر عبر الـ Team Lead.
