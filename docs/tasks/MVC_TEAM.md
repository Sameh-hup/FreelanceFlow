# 📌 مهام فريق واجهة الويب (MVC & Front-End)

مرحباً يا أبطال! الـ Controllers والـ Layout مهيأة بالكامل، وتقدروا تبدأوا في تصميم الشاشات وربطها بالـ Services مباشرة.

---

## 📂 الشاشات والـ Controllers المطلوب إكمالها:

### 1. شاشات الحسابات (`AccountController`):
- **`Views/Account/Login.cshtml`**: نموذج تسجيل الدخول مع إظهار أخطاء المصادقة.
- **`Views/Account/Register.cshtml`**: تسجيل خاص بالفريالنسر فقط.
- **`Views/Account/ChangePassword.cshtml`**: إجبار الـ Admin على تغيير كلمة السر عند أول دخول.

---

### 2. شاشات العملاء (`ClientsController`):
- **`Index.cshtml`**: جدول العملاء مع شريط بحث بالاسم، وزر إضافة، وزر توليد دعوة ينسخ الرابط بضغطة زر.
- **`Create.cshtml`** و **`Edit.cshtml`**: فورم بيانات العميل (اسم الشركة، الهاتف، العنوان، الملاحظات).
- **`Details.cshtml`**: بيانات العميل وقائمة مشاريعه.

---

### 3. شاشات المشاريع (`ProjectsController`):
- **`Index.cshtml`**: كروت أو جدول المشاريع مع شريط بحث بالاسم، وإظهار الـ **Health Badge** بلون واضح (`Critical` أحمر، `AttentionRequired` أصفر، `Healthy` أخضر) وشريط تقدم Progress Bar.
- **`Create.cshtml`**: اختيار العميل من Dropdown، الاسم، الميزانية، العملة، تاريخ البداية والـ Deadline.
- **`Details.cshtml`**: الشاشة الرئيسية للمشروع؛ تعرض:
  - تفاصيل المشروع والميزانية والـ Progress.
  - المراحل (Milestones) والمهام التابعة لها.
  - إمكانية تغيير حالة المهمة تفاعلياً (`Pending` / `InProgress` / `Completed`).
  - الفواتير المرتبطة بالمشروع.

---

### 4. شاشات الفواتير والمدفوعات (`InvoicesController`):
- **`Index.cshtml`**: قائمة الفواتير مع شريط بحث بالاسم وحالة الفاتورة (Paid, Overdue, PartiallyPaid, Unpaid).
- **`Create.cshtml`**: إنشاء فاتورة لمشروع محدد.
- **`Details.cshtml`**: تفاصيل الفاتورة، الدفعات السابقة، الرصيد المتبقي، وزر لفتح **Modal تسجيل دفعة جديدة**.

---

### 5. لوحة الأدمن وبوابة العميل (`AdminController` & `PortalController`):
- **`Views/Admin/Index.cshtml`**: قائمة المستخدمين مع أزرار تفعيل/تجميد الحساب (Toggle Status)، بدون عرض أي بيانات مالية.
- **`Views/Portal/Index.cshtml`**: شاشة القراءة فقط للعميل لمتابعة تقدم مشاريعه وفواتيره.

---

## 🎨 تعليمات التصميم:
- الاعتماد على **Bootstrap 5** و **Bootstrap Icons** (تم تضمينهم في `_Layout.cshtml`).
- استخدام الـ Partial View `_Alerts.cshtml` لعرض رسائل النجاح والخطأ من `TempData["SuccessMessage"]`.
- إنشاء الـ ViewModels الخاصة بالنماذج داخل مجلد `App.Mvc/Models/`.
