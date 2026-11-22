# 🛡️ دليل التيم ليدر (Team Lead Guide - Member 5)

أهلاً يا هندسة! هذا دليلك الشامل لإدارة طبقة الـ **DAL**، تجميع وربط أجزاء المشروع، ومراجعة عمل الفريق لضمان التسليم بنجاح بأعلى معايير الجودة.

تم إعداد **وثيقة الدليل الشامل للتيم ليدر باللغة الإنجليزية** على نفس نسق وطريقة ملف الـ Assignment الخاص بمشروع `CityLibrary`.

🔗 **المرجع الفني الأساسي المطلوب قراءته والاعتماد عليه:**
👉 [TEAM_LEAD_MASTER_GUIDE.md](file:///d:/Full%20stack%20course/freelancer%20Flow%20Project/FreelancerFlowApp/docs/tasks/TEAM_LEAD_MASTER_GUIDE.md)

---

## 🧭 مهامك الأساسية الثلاث كـ Team Lead:

### 1. إدارة طبقة البيانات (`App.DAL`):
* أنت المرجع الوحيد لـ `AppDbContext` والـ Entities والـ Migrations.
* تطبيق جميع الـ Configurations المنفصلة وتحديد العلاقات:
  * Cascade Delete: `Tasks → Milestones → Projects`.
  * Restrict Delete: لجميع العلاقات الأخرى (منع حذف عميل أو مشروع عليه فواتير أو فاتورة عليها دفعات).
  * Concurrency Token: تفعيل `builder.Property(i => i.RowVersion).IsRowVersion()` لحماية الفواتير والدفعات المتزامنة.
* إدارة الـ Migrations والـ Seeding (`DbInitializer.cs`) لزرع حساب الأدمن وبيانات الاختبار التوضيحية (Section 11).

---

### 2. التجميع والربط الشامل (System Assembly & Wiring):
* **ملف `Program.cs` في `App.Mvc`**:
  * ربط قاعدة البيانات مع سلسلة الاتصال `DefaultConnection`.
  * إعداد خدمة الـ Identity والكوكيز (`ExpireTimeSpan`, `LoginPath = "/Account/Login"`, إلخ).
  * تسجيل خدمات الـ BLL عبر `builder.Services.AddBLL()`.
  * ترتيب الـ Middleware الحرج: `app.UseAuthentication()` قبل `app.UseAuthorization()`.
  * تنفيذ التحديث التلقائي لقاعدة البيانات والزرع `DbInitializer.SeedAsync()` عند بدء التشغيل داخل `CreateScope`.

---

### 3. مراجعة الكود والـ Pull Requests (القواعد الذهبية العشر):
قبل قبول ودمج أي PR من أعضاء الفريق الأربعة في فرع `develop`، تأكد من:
1. البناء النظيف (`0 Errors, 0 Warnings`).
2. عدم وصول الـ MVC إلى `AppDbContext` نهائياً.
3. عدم وجود أي كلمة `DateTime.Now`، والاعتماد الحصري على `IClock`.
4. استخدام نوع `decimal` حصراً في جميع القيم والعمليات المالية.
5. خوارزميات الحسابات (Progress, Health, Status) نقية ومغطاة باختبارات Unit Tests.
6. تسجيل الدفعات يتم داخل Transaction ويعالج استثناء التزامن `DbUpdateConcurrencyException`.
7. حذف العملاء يتم كحذف ناعم `IsDeleted = true`.
8. وجود `[ValidateAntiForgeryToken]` و `@Html.AntiForgeryToken()` في كل طلب POST.
9. خلو شاشات ولوحة الأدمن من أي بيانات مالية (BR-28).
10. معالجة القوائم الفارغة (Empty States) ببطاقات تنبيهية بدلاً من الجداول الفارغة.

---

## 🚀 سيناريو التحقق النهائي للتسليم (End-to-End Walkthrough):
مفصل بالكامل داخل [TEAM_LEAD_MASTER_GUIDE.md](file:///d:/Full%20stack%20course/freelancer%20Flow%20Project/FreelancerFlowApp/docs/tasks/TEAM_LEAD_MASTER_GUIDE.md) مع مخطط تسلسلي (Sequence Diagram) يغطي تجربة كل دور من الأدوار الثلاثة خطوة بخطوة.
