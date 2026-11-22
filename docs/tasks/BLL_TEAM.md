# 📌 دليل ومهام فريق الـ Business Logic Layer (BLL)

مرحباً يا شباب! البنية التحتية والـ Interfaces جاهزة بالكامل عشان تبدأوا كتابة الكود مباشرة.

تم إعداد **وثيقة المواصفات الفنية الكاملة للمشروع باللغة الإنجليزية** على نفس نسق وطريقة ملف الـ Assignment الخاص بمشروع `CityLibrary`.

🔗 **المرجع الأساسي المطلوب قراءته والالتزام به بدقة:**
👉 [BLL_SPECIFICATION.md](file:///d:/Full%20stack%20course/freelancer%20Flow%20Project/FreelancerFlowApp/docs/tasks/BLL_SPECIFICATION.md)

---

## 👥 تقسيم المهام بين عضوي فريق الـ BLL:

### 1. Developer 1 (إدارة النطاق ودورة حياة المشاريع والمهام):
- **الـ Repositories:**
  - `ClientRepository.cs` (تطبيق `IClientRepository`)
  - `ProjectRepository.cs` (تطبيق `IProjectRepository`)
- **الـ Services:**
  - `ClientService.cs`: تطبيق الـ CRUD وضمان الـ Soft Delete.
  - `ProjectService.cs`: دورة حياة المشروع، والتأكد من عدم حذف مشروع لديه فواتير.
  - `TaskService.cs`: إضافة المراحل والمهام ومطابقة قيود التواريخ (BR-01, BR-02, BR-03).
- **خوارزميات الحسابات النقية (Pure Calculations):**
  - حساب نسبة الإنجاز: `CalculateProgress` (قاعدة BR-10).
  - تحديد المهام المتأخرة: `GetOverdueTasksAsync` (قاعدة BR-11).
  - تحديد حالة المرحلة: `CalculateMilestoneStatus` (قاعدة BR-12).
  - تقييم صحة المشروع: `CalculateHealth` (جدول Section 8 وقاعدة BR-31).
- **الـ Validations:**
  - `ClientValidator.cs`، `ProjectValidator.cs`، `TaskValidator.cs`.

---

### 2. Developer 2 (العمليات المالية، التزامن، والأمان):
- **الـ Repositories:**
  - `InvoiceRepository.cs` (تطبيق `IInvoiceRepository`)
- **الـ Services:**
  - `InvoiceService.cs`:
    - إنشاء وحذف الفواتير (منع حذف فاتورة عليها دفعات BR-21).
    - تسجيل الدفعة داخل Transaction ذرية والتحقق من عدم تجاوز الرصيد المتبقي (BR-18, BR-19).
    - معالجة تضارب البيانات المتزامنة عبر `RowVersion` وإلقاء رسالة واضحة للمستخدم (NFR-11).
    - تسجيل حركة الـ `AuditLog` في نفس العملية (BR-29).
  - `AuthService.cs`:
    - تسجيل حسابات الفريلانسر فقط عبر التسجيل العام (FR-01).
    - توليد توكن الدعوة (عشوائي، تشفير SHA256، مدة 7 أيام BR-25).
    - تسجيل حساب العميل عبر كود الدعوة وربطه بسجل العميل.
  - `AccessService.cs`:
    - فحص الملكية والصلاحيات المركزية (BR-27).
- **خوارزميات الحسابات المالية:**
  - حساب الرصيد المتبقي: `CalculateRemainingBalance` (قاعدة BR-17).
  - حساب حالة الفاتورة: `CalculateInvoiceStatus` بالأولويات الأربعة (قاعدة BR-16).
- **الـ Validations:**
  - `InvoiceValidator.cs`، `PaymentValidator.cs`.

---

## ⚠️ تنبيهات هامة جداً:
1. **التواريخ**: ممنوع منعاً باتاً كتابة `DateTime.Now` في أي مكان. استخدموا `IClock` (`_clock.UtcNow` للتخزين، و `_clock.CairoToday` للمقارنات).
2. **البيانات المالية**: جميع المبالغ من نوع `decimal` حصراً.
3. **عدم كتابة كود مكتمل**: الوثيقة تشرح القواعد والشروط والمعايير (Signatures, Validation Tables, Rules)، ومطلوب من كل مطور كتابة الكود بنفسه وفق الدليل.
4. **التسجيل بالـ DI**: يتم تسجيل جميع الـ Services والـ Repositories داخل `BllServiceExtensions.cs`.
