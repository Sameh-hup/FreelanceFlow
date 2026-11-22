# 📌 دليل ومهام فريق واجهة الويب (MVC & Front-End)

مرحباً يا شباب! البنية التحتية والـ Controllers والـ Layout مهيأة بالكامل لتبدأوا في تصميم وبناء الشاشات وربطها بالـ Services.

تم إعداد **وثيقة المواصفات الفنية الكاملة لطبقة الـ MVC باللغة الإنجليزية** على نفس نسق وطريقة ملف الـ Assignment الخاص بمشروع `CityLibrary`.

🔗 **المرجع الأساسي المطلوب قراءته والالتزام به بدقة:**
👉 [MVC_SPECIFICATION.md](file:///d:/Full%20stack%20course/freelancer%20Flow%20Project/FreelancerFlowApp/docs/tasks/MVC_SPECIFICATION.md)

---

## 👥 تقسيم المهام بين عضوي فريق الـ MVC:

### 1. Developer 1 (الأمان، القوالب، البوابات، ولوحة التحكم):
- **الـ Shell و Navigation:**
  - `Views/Shared/_Layout.cshtml`: جعل شريط التنقل ديناميكياً بحسب دور المستخدم (زائر، فريلانسر، عميل، أدمن).
- **إدارة الحسابات والمصادقة (`AccountController`):**
  - `Login.cshtml`: تسجيل الدخول مع دعم `ReturnUrl` وتوجيه كل مستخدم للوحة التحكم الخاصة به.
  - `Register.cshtml`: التسجيل العام للفريلانسر فقط (FR-01).
  - `ChangePassword.cshtml`: شاشة إجبار الأدمن على تغيير كلمة المرور عند أول دخول (FR-04).
  - `RegisterByInvitation.cshtml`: شاشة تسجيل العميل عن طريق رابط الدعوة المحتوي على التوكن (FR-05, BR-25).
  - `AccessDenied.cshtml`: صفحة الخطأ 403 بشكل منظم.
- **لوحة التحكم الرئيسية للفريلانسر (`HomeController`):**
  - `Views/Home/Index.cshtml`: الـ Dashboard التي تعرض بالضبط 4 كروت إحصائية (عدد العملاء، المشاريع النشطة، المهام المتأخرة، الفواتير غير المدفوعة) طبقاً لقاعدة (FR-29).
- **لوحة الأدمن وبوابة العميل:**
  - `Views/Admin/Index.cshtml`: عرض المستخدمين مع زر تفعيل/إيقاف الحساب، وممنوع عرض أي بيانات مالية (BR-28).
  - `Views/Portal/Index.cshtml`: بوابة العميل للقراءة فقط (متابعة مشاريعه، نسب الإنجاز، وفواتيره) طبقاً لقواعد (FR-27, FR-28).
- **نظام التنبيهات (Toast Notifications):**
  - ربط الـ `TosterViewModel` مع الـ Partial View `_TosterPartial.cshtml`.

---

### 2. Developer 2 (وحدات العمل الأساسية: العملاء، المشاريع، المهام، والفواتير):
- **الـ Form Partials القابلة لإعادة الاستخدام (Reusable Partials):**
  - `_ClientFormFieldsPartial.cshtml`
  - `_ProjectFormFieldsPartial.cshtml`
  - `_InvoiceFormFieldsPartial.cshtml`
- **شاشات العملاء (`ClientsController`):**
  - `Index.cshtml`: جدول/كروت العملاء مع البحث، وزر فتح Modal لتوليد رابط الدعوة ونسخه بضغطة زر (FR-05).
  - `Create.cshtml` و `Edit.cshtml`: نماذج الإضافة والتعديل باستخدام الـ Partial المشترك.
  - `Details.cshtml`: الملف التعريفي للعميل مع قائمة مشاريعه.
- **شاشات المشاريع والمهام (`ProjectsController`, `TasksController`):**
  - `Index.cshtml`: كروت المشاريع مع شريط التقدم Progress Bar وألوان الـ Health Badges الثلاثة (`Healthy` أخضر، `AttentionRequired` أصفر، `Critical` أحمر).
  - `Create.cshtml`: إنشاء المشروع مع قائمة منسدلة لاختيار العميل، وتحديد العملة والتواريخ.
  - `Details.cshtml`: شاشة مساحة العمل الرئيسية؛ تعرض Accordion للمراحل (Milestones)، وشجرة المهام التابعة لها مع إمكانية تغيير حالة المهمة (`Pending` / `InProgress` / `Completed`) وتنبيه أيقونة حمراء للمهام المتأخرة Overdue.
- **شاشات الفواتير والمدفوعات (`InvoicesController`):**
  - `Index.cshtml`: قائمة الفواتير مع شارات الحالة (Paid, Overdue, PartiallyPaid, Unpaid).
  - `Details.cshtml`: تفاصيل الفاتورة، جدول الدفعات السابقة، الرصيد المتبقي، وزر لفتح **Modal تسجيل دفعة جديدة**.
  - معالجة رسائل الخطأ والتزامن Concurrency (`DbUpdateConcurrencyException`) ومنع حذف الفواتير ذات الدفعات (BR-21).

---

## ⚠️ تعليمات هامة:
1. **الفصل المعماري**: الـ Controllers تستدعي الـ Services من الـ BLL فقط، وممنوع استخدام `AppDbContext` داخل الـ MVC نهائياً.
2. **الحماية ضد الـ CSRF**: كل فورم يرسل POST يجب أن يحتوي على `@Html.AntiForgeryToken()` وأكشن الـ Controller يحمل `[ValidateAntiForgeryToken]`.
3. **الـ Empty States**: إذا كانت القائمة فارغة، اعرض بطاقة أو Alert لطيف في Bootstrap مع زر إضافة، ولا تترك الجدول فارغاً برؤوس أعمدة فقط.
4. **التنبيهات**: استخدم `TempData["ToastMessage"]` بعد كل عملية تحويل (Post-Redirect-Get).
