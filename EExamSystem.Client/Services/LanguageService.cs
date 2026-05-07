namespace EExamSystem.Client.Services;

public class LanguageService
{
    public string Current { get; private set; } = "en";
    public bool IsRtl => Current == "ar";
    public event Action? OnChange;

    public void SetLanguage(string lang)
    {
        Current = lang;
        OnChange?.Invoke();
    }

    public string T(string key) =>
        translations.TryGetValue((Current, key), out var val) ? val : key;

    private readonly Dictionary<(string, string), string> translations = new()
    {
        // --- Navbar ---
        { ("en", "nav.login"),      "Login" },
        { ("en", "nav.logout"),     "Logout" },
        { ("en", "nav.exams"),      "Exams" },
        { ("en", "nav.instructor"), "Instructor" },
        { ("en", "nav.admin"),      "Admin" },

        { ("ar", "nav.login"),      "تسجيل الدخول" },
        { ("ar", "nav.logout"),     "تسجيل الخروج" },
        { ("ar", "nav.exams"),      "الاختبارات" },
        { ("ar", "nav.instructor"), "المحاضر" },
        { ("ar", "nav.admin"),      "الإدارة" },

        // --- Login ---
        { ("en", "login.welcome"),       "Welcome Back" },
        { ("en", "login.subtitle"),      "Sign in to your EExam account" },
        { ("en", "login.email"),         "Email" },
        { ("en", "login.email.ph"),      "you@university.edu" },
        { ("en", "login.password"),      "Password" },
        { ("en", "login.submit"),        "Sign In" },
        { ("en", "login.loading"),       "Signing in..." },
        { ("en", "login.err.empty"),     "Please enter your email and password." },
        { ("en", "login.err.invalid"),   "Invalid credentials." },
        { ("en", "login.err.locked"),    "Account locked. Too many failed attempts. Try again later." },

        { ("ar", "login.welcome"),       "مرحباً بعودتك" },
        { ("ar", "login.subtitle"),      "سجّل دخولك إلى نظام الاختبارات الإلكترونية" },
        { ("ar", "login.email"),         "البريد الإلكتروني" },
        { ("ar", "login.email.ph"),      "you@university.edu" },
        { ("ar", "login.password"),      "كلمة المرور" },
        { ("ar", "login.submit"),        "تسجيل الدخول" },
        { ("ar", "login.loading"),       "جارٍ تسجيل الدخول..." },
        { ("ar", "login.err.empty"),     "يرجى إدخال البريد الإلكتروني وكلمة المرور." },
        { ("ar", "login.err.invalid"),   "بيانات غير صحيحة." },
        { ("ar", "login.err.locked"),    "الحساب مقفل. تجاوزت عدد محاولات تسجيل الدخول. حاول لاحقاً." },

        // --- Exams ---
        { ("en", "exams.title"),    "My Exams" },
        { ("en", "exams.subtitle"), "View and take your scheduled exams" },
        { ("en", "exams.start"),    "Start Exam" },
        { ("en", "exams.tab.all"),       "All" },
        { ("en", "exams.tab.available"), "Available" },

        { ("ar", "exams.title"),    "اختباراتي" },
        { ("ar", "exams.subtitle"), "عرض اختباراتك المجدولة وأدائها" },
        { ("ar", "exams.start"),    "ابدأ الاختبار" },
        { ("ar", "exams.tab.all"),       "الكل" },
        { ("ar", "exams.tab.available"), "متاح" },

        // --- Admin ---
        { ("en", "admin.title"),    "Admin Dashboard" },
        { ("en", "admin.subtitle"), "System overview and management" },
        { ("en", "admin.add_user"), "+ Add User" },

        { ("ar", "admin.title"),    "لوحة الإدارة" },
        { ("ar", "admin.subtitle"), "نظرة عامة على النظام وإدارته" },
        { ("ar", "admin.add_user"), "+ إضافة مستخدم" },

        // --- Instructor ---
        { ("en", "instructor.title"),    "My Courses" },
        { ("en", "instructor.subtitle"), "Manage question banks for your courses" },

        { ("ar", "instructor.title"),    "مقرراتي" },
        { ("ar", "instructor.subtitle"), "إدارة بنوك الأسئلة لمقرراتك" },

        // --- Policy ---
        { ("en", "policy.title"),    "Terms of Use & Privacy Policy" },
        { ("en", "policy.p1"),       "By logging in, you agree to the EExam System's Terms of Use. This platform is intended solely for authorized students, instructors, and administrators of King Abdulaziz University. Unauthorized access is strictly prohibited and may be subject to disciplinary or legal action." },
        { ("en", "policy.p2"),       "All examination activity is monitored and logged for academic integrity purposes. Your personal data is handled in accordance with the University's Privacy Policy and applicable data protection regulations. For support or inquiries, contact the IT Help Desk." },
        { ("en", "policy.copy"),     "King Abdulaziz University — EExam System. All rights reserved." },

        { ("ar", "policy.title"),    "شروط الاستخدام وسياسة الخصوصية" },
        { ("ar", "policy.p1"),       "بتسجيل دخولك، فإنك توافق على شروط استخدام نظام الاختبارات الإلكترونية. هذه المنصة مخصصة حصراً للطلاب والمحاضرين والمسؤولين المعتمدين في جامعة الملك عبدالعزيز. يُحظر الوصول غير المصرح به ويخضع للمساءلة التأديبية أو القانونية." },
        { ("ar", "policy.p2"),       "تُراقَب جميع أنشطة الاختبارات وتُسجَّل لأغراض النزاهة الأكاديمية. يُعالَج تعريف بياناتك الشخصية وفقاً لسياسة الخصوصية الجامعية والأنظمة المعمول بها لحماية البيانات. للدعم أو الاستفسار، تواصل مع مكتب تقنية المعلومات." },
        { ("ar", "policy.copy"),     "جامعة الملك عبدالعزيز — نظام الاختبارات الإلكترونية. جميع الحقوق محفوظة." },

        // --- Banks ---
        { ("en", "banks.title"),      "Question Banks" },
        { ("en", "banks.new"),        "+ New Bank" },
        { ("en", "banks.create"),     "Create New Bank" },
        { ("en", "banks.ph"),         "e.g. Chapter 3 — Sorting Algorithms" },
        { ("en", "banks.btn.create"), "Create" },
        { ("en", "banks.btn.cancel"), "Cancel" },

        { ("ar", "banks.title"),      "بنوك الأسئلة" },
        { ("ar", "banks.new"),        "+ بنك جديد" },
        { ("ar", "banks.create"),     "إنشاء بنك جديد" },
        { ("ar", "banks.ph"),         "مثال: الفصل 3 — خوارزميات الترتيب" },
        { ("ar", "banks.btn.create"), "إنشاء" },
        { ("ar", "banks.btn.cancel"), "إلغاء" },

        // --- General actions ---
        { ("en", "action.save"),    "Save" },
        { ("en", "action.cancel"),  "Cancel" },
        { ("en", "action.delete"),  "Delete" },
        { ("en", "action.edit"),    "Edit" },
        { ("en", "action.create"),  "Create" },
        { ("en", "action.confirm"), "Are you sure?" },
        { ("en", "action.yes"),     "Yes" },
        { ("en", "action.no"),      "No" },
        { ("en", "err.load"),       "Failed to load data. Please refresh." },
        { ("en", "err.save"),       "Failed to save. Please try again." },

        { ("ar", "action.save"),    "حفظ" },
        { ("ar", "action.cancel"),  "إلغاء" },
        { ("ar", "action.delete"),  "حذف" },
        { ("ar", "action.edit"),    "تعديل" },
        { ("ar", "action.create"),  "إنشاء" },
        { ("ar", "action.confirm"), "هل أنت متأكد؟" },
        { ("ar", "action.yes"),     "نعم" },
        { ("ar", "action.no"),      "لا" },
        { ("ar", "err.load"),       "فشل تحميل البيانات. يرجى تحديث الصفحة." },
        { ("ar", "err.save"),       "فشل الحفظ. يرجى المحاولة مجددًا." },

        // --- Navbar admin sub-links ---
        { ("en", "nav.users"),   "Users" },
        { ("en", "nav.courses"), "Courses" },
        { ("ar", "nav.users"),   "المستخدمون" },
        { ("ar", "nav.courses"), "المقررات" },

        // --- Admin: User management ---
        { ("en", "admin.users.title"),    "User Management" },
        { ("en", "admin.users.subtitle"), "Create, manage, and assign roles to users" },
        { ("en", "admin.users.create"),   "+ New User" },
        { ("en", "admin.users.fullname"), "Full Name" },
        { ("en", "admin.users.email"),    "Email" },
        { ("en", "admin.users.password"), "Password" },
        { ("en", "admin.users.roles"),    "Roles" },
        { ("en", "admin.users.empty"),    "No users found. Create the first one above." },

        { ("ar", "admin.users.title"),    "إدارة المستخدمين" },
        { ("ar", "admin.users.subtitle"), "إنشاء المستخدمين وإدارتهم وتعيين الأدوار" },
        { ("ar", "admin.users.create"),   "+ مستخدم جديد" },
        { ("ar", "admin.users.fullname"), "الاسم الكامل" },
        { ("ar", "admin.users.email"),    "البريد الإلكتروني" },
        { ("ar", "admin.users.password"), "كلمة المرور" },
        { ("ar", "admin.users.roles"),    "الأدوار" },
        { ("ar", "admin.users.empty"),    "لا يوجد مستخدمون. أنشئ الأول أعلاه." },

        // --- Admin: Course management ---
        { ("en", "admin.courses.title"),    "Course Management" },
        { ("en", "admin.courses.subtitle"), "Create and manage university courses" },
        { ("en", "admin.courses.create"),   "+ New Course" },
        { ("en", "admin.courses.name"),     "Course Name" },
        { ("en", "admin.courses.code"),     "Course Code" },
        { ("en", "admin.courses.code.ph"),  "e.g. CPCS202" },
        { ("en", "admin.courses.empty"),    "No courses yet. Add the first course above." },

        { ("ar", "admin.courses.title"),    "إدارة المقررات" },
        { ("ar", "admin.courses.subtitle"), "إنشاء المقررات الجامعية وإدارتها" },
        { ("ar", "admin.courses.create"),   "+ مقرر جديد" },
        { ("ar", "admin.courses.name"),     "اسم المقرر" },
        { ("ar", "admin.courses.code"),     "رمز المقرر" },
        { ("ar", "admin.courses.code.ph"),  "مثال: CPCS202" },
        { ("ar", "admin.courses.empty"),    "لا توجد مقررات بعد. أضف الأول أعلاه." },

        // --- Sections ---
        { ("en", "sections.title"),    "Sections" },
        { ("en", "sections.subtitle"), "Manage class sections and student enrollment" },
        { ("en", "sections.create"),   "+ New Section" },
        { ("en", "sections.name"),     "Section Name" },
        { ("en", "sections.name.ph"),  "e.g. Section AA" },
        { ("en", "sections.empty"),    "No sections yet. Create one above." },
        { ("en", "sections.enroll"),   "Enroll Student by ID" },
        { ("en", "sections.studentid"),"Student ID" },

        { ("ar", "sections.title"),    "الشعب" },
        { ("ar", "sections.subtitle"), "إدارة الشعب الدراسية وتسجيل الطلاب" },
        { ("ar", "sections.create"),   "+ شعبة جديدة" },
        { ("ar", "sections.name"),     "اسم الشعبة" },
        { ("ar", "sections.name.ph"),  "مثال: الشعبة أ" },
        { ("ar", "sections.empty"),    "لا توجد شعب بعد. أنشئ واحدة أعلاه." },
        { ("ar", "sections.enroll"),   "تسجيل طالب بالمعرف" },
        { ("ar", "sections.studentid"),"معرّف الطالب" },

        // --- Exams management (instructor) ---
        { ("en", "exams.manage.title"),    "Exams" },
        { ("en", "exams.manage.subtitle"), "Create and manage exams for this course" },
        { ("en", "exams.create"),          "+ New Exam" },
        { ("en", "exams.title.label"),     "Exam Title" },
        { ("en", "exams.starttime"),        "Start Time" },
        { ("en", "exams.endtime"),         "End Time" },
        { ("en", "exams.duration"),        "Duration (min)" },
        { ("en", "exams.maxscore"),        "Max Score" },
        { ("en", "exams.passingscore"),    "Passing Score" },
        { ("en", "exams.empty"),           "No exams yet for this course. Create one above." },
        { ("en", "exams.sections"),        "Assigned Sections" },
        { ("en", "exams.assign"),          "Assign to Section" },

        { ("ar", "exams.manage.title"),    "الاختبارات" },
        { ("ar", "exams.manage.subtitle"), "إنشاء الاختبارات وإدارتها لهذا المقرر" },
        { ("ar", "exams.create"),          "+ اختبار جديد" },
        { ("ar", "exams.title.label"),     "عنوان الاختبار" },
        { ("ar", "exams.starttime"),        "وقت البدء" },
        { ("ar", "exams.endtime"),         "وقت الانتهاء" },
        { ("ar", "exams.duration"),        "المدة (دقيقة)" },
        { ("ar", "exams.maxscore"),        "الدرجة القصوى" },
        { ("ar", "exams.passingscore"),    "درجة النجاح" },
        { ("ar", "exams.empty"),           "لا توجد اختبارات لهذا المقرر بعد. أنشئ واحدًا أعلاه." },
        { ("ar", "exams.sections"),        "الشعب المعيّنة" },
        { ("ar", "exams.assign"),          "تعيين لشعبة" },

        // --- Common ---
        { ("en", "common.loading"),        "Loading..." },
        { ("en", "common.loading.exam"),   "Loading exam..." },
        { ("en", "common.loading.exams"),  "Loading exams..." },
        { ("en", "common.loading.users"),  "Loading users..." },
        { ("en", "common.loading.courses"),"Loading courses..." },
        { ("en", "common.loading.sections"),"Loading sections..." },
        { ("en", "common.loading.banks"),  "Loading..." },
        { ("en", "common.loading.students"),"Loading students..." },
        { ("en", "common.loading.testbanks"),"Loading testbanks..." },
        { ("en", "common.select"),         "Select..." },
        { ("en", "common.add"),            "Add" },
        { ("en", "common.min"),            "min" },
        { ("en", "common.back.exams"),     "← Back to My Exams" },

        { ("ar", "common.loading"),        "جارٍ التحميل..." },
        { ("ar", "common.loading.exam"),   "جارٍ تحميل الاختبار..." },
        { ("ar", "common.loading.exams"),  "جارٍ تحميل الاختبارات..." },
        { ("ar", "common.loading.users"),  "جارٍ تحميل المستخدمين..." },
        { ("ar", "common.loading.courses"),"جارٍ تحميل المقررات..." },
        { ("ar", "common.loading.sections"),"جارٍ تحميل الشعب..." },
        { ("ar", "common.loading.banks"),  "جارٍ التحميل..." },
        { ("ar", "common.loading.students"),"جارٍ تحميل الطلاب..." },
        { ("ar", "common.loading.testbanks"),"جارٍ تحميل بنوك الأسئلة..." },
        { ("ar", "common.select"),         "اختر..." },
        { ("ar", "common.add"),            "إضافة" },
        { ("ar", "common.min"),            "دقيقة" },
        { ("ar", "common.back.exams"),     "← العودة إلى اختباراتي" },

        // --- Take Exam ---
        { ("en", "exam.notfound"),         "Exam not found or you are not enrolled in this exam." },
        { ("en", "exam.detail.duration"),  "Duration" },
        { ("en", "exam.detail.questions"), "Questions" },
        { ("en", "exam.detail.maxscore"),  "Max Score" },
        { ("en", "exam.detail.passing"),   "Passing Score" },
        { ("en", "exam.detail.schedule"),  "Schedule" },
        { ("en", "exam.noquestions"),      "No questions have been assigned to this exam yet." },
        { ("en", "exam.start.btn"),        "Start Exam →" },
        { ("en", "exam.question.of"),      "Question {0} of {1}" },
        { ("en", "exam.remaining"),        "remaining" },
        { ("en", "exam.prev"),             "← Previous" },
        { ("en", "exam.next"),             "Next →" },
        { ("en", "exam.submit"),           "Submit" },
        { ("en", "exam.passed"),           "Passed!" },
        { ("en", "exam.notpassed"),        "Not Passed" },
        { ("en", "exam.result.score"),     "Score" },
        { ("en", "exam.result.max"),       "Max" },
        { ("en", "exam.result.pass"),      "Pass" },
        { ("en", "exam.result.answered"),  "You answered {0} out of {1} questions correctly." },

        { ("ar", "exam.notfound"),         "الاختبار غير موجود أو أنت غير مسجل في هذا الاختبار." },
        { ("ar", "exam.detail.duration"),  "المدة" },
        { ("ar", "exam.detail.questions"), "الأسئلة" },
        { ("ar", "exam.detail.maxscore"),  "الدرجة الكاملة" },
        { ("ar", "exam.detail.passing"),   "درجة النجاح" },
        { ("ar", "exam.detail.schedule"),  "الجدول" },
        { ("ar", "exam.noquestions"),      "لم تُعيَّن أسئلة لهذا الاختبار بعد." },
        { ("ar", "exam.start.btn"),        "ابدأ الاختبار ←" },
        { ("ar", "exam.question.of"),      "سؤال {0} من {1}" },
        { ("ar", "exam.remaining"),        "متبقٍّ" },
        { ("ar", "exam.prev"),             "← السابق" },
        { ("ar", "exam.next"),             "التالي →" },
        { ("ar", "exam.submit"),           "تسليم" },
        { ("ar", "exam.passed"),           "ناجح!" },
        { ("ar", "exam.notpassed"),        "غير ناجح" },
        { ("ar", "exam.result.score"),     "الدرجة" },
        { ("ar", "exam.result.max"),       "الأقصى" },
        { ("ar", "exam.result.pass"),      "النجاح" },
        { ("ar", "exam.result.answered"),  "أجبت على {0} من {1} سؤالاً بشكل صحيح." },

        // --- Student Exams list ---
        { ("en", "exams.noactive"),        "No active exams at the moment." },
        { ("en", "exams.nohistory"),       "No exam history yet." },
        { ("en", "exams.inprogress"),      "In progress" },

        { ("ar", "exams.noactive"),        "لا توجد اختبارات نشطة في الوقت الحالي." },
        { ("ar", "exams.nohistory"),       "لا يوجد سجل اختبارات بعد." },
        { ("ar", "exams.inprogress"),      "قيد التنفيذ" },

        // --- Course Exams (instructor) ---
        { ("en", "exams.form.new"),        "New Exam" },
        { ("en", "exams.form.edit"),       "Edit Exam" },
        { ("en", "exams.title.ph"),        "e.g. Midterm Exam" },
        { ("en", "exams.chapters.label"),  "Question Banks — select chapters to include" },
        { ("en", "exams.chapters.empty"),  "No testbanks or chapters found. Create them first in the Banks section." },
        { ("en", "exams.chapter.singular"),"chapter" },
        { ("en", "exams.chapter.plural"),  "chapters" },
        { ("en", "exams.chapters.selected"),"{0} chapter(s) selected" },
        { ("en", "exams.nochapters.bank"), "No chapters in this bank." },
        { ("en", "exams.nosections"),      "No sections available. Create sections first." },

        { ("ar", "exams.form.new"),        "اختبار جديد" },
        { ("ar", "exams.form.edit"),       "تعديل الاختبار" },
        { ("ar", "exams.title.ph"),        "مثال: اختبار منتصف الفصل" },
        { ("ar", "exams.chapters.label"),  "بنوك الأسئلة — اختر الفصول المراد تضمينها" },
        { ("ar", "exams.chapters.empty"),  "لا توجد بنوك أسئلة أو فصول. أنشئها أولاً في قسم البنوك." },
        { ("ar", "exams.chapter.singular"),"فصل" },
        { ("ar", "exams.chapter.plural"),  "فصول" },
        { ("ar", "exams.chapters.selected"),"{0} فصل/فصول مختارة" },
        { ("ar", "exams.nochapters.bank"), "لا توجد فصول في هذا البنك." },
        { ("ar", "exams.nosections"),      "لا توجد شعب. أنشئ الشعب أولاً." },

        // --- Sections ---
        { ("en", "sections.form.new"),     "New Section" },
        { ("en", "sections.loading.sections"), "Loading sections..." },
        { ("en", "sections.id"),           "Section ID: {0}" },
        { ("en", "sections.enroll.btn"),   "Enroll Student" },
        { ("en", "sections.enroll.header"),"Enroll a Student" },
        { ("en", "sections.no.students"),  "No students available to enroll." },
        { ("en", "sections.select.student"),"— Select a student —" },
        { ("en", "sections.enroll.action"),"Enroll" },

        { ("ar", "sections.form.new"),     "شعبة جديدة" },
        { ("ar", "sections.loading.sections"), "جارٍ تحميل الشعب..." },
        { ("ar", "sections.id"),           "معرّف الشعبة: {0}" },
        { ("ar", "sections.enroll.btn"),   "تسجيل طالب" },
        { ("ar", "sections.enroll.header"),"تسجيل طالب" },
        { ("ar", "sections.no.students"),  "لا يوجد طلاب متاحون للتسجيل." },
        { ("ar", "sections.select.student"),"— اختر طالباً —" },
        { ("ar", "sections.enroll.action"),"تسجيل" },

        // --- Banks ---
        { ("en", "banks.empty.msg"),       "No question banks yet." },
        { ("en", "banks.empty.hint"),      "Create one using the button above." },
        { ("en", "banks.created.on"),      "Created" },

        { ("ar", "banks.empty.msg"),       "لا توجد بنوك أسئلة بعد." },
        { ("ar", "banks.empty.hint"),      "أنشئ واحداً باستخدام الزر أعلاه." },
        { ("ar", "banks.created.on"),      "تاريخ الإنشاء" },

        // --- Bank Editor ---
        { ("en", "editor.add.question"),   "+ Add Question" },
        { ("en", "editor.add.chapter"),    "+ Chapter" },
        { ("en", "editor.chapter.ph"),     "Chapter name..." },
        { ("en", "editor.select.chapter"), "Select a chapter to view and manage questions." },
        { ("en", "editor.no.chapters"),    "No chapters yet." },
        { ("en", "editor.no.chapters.hint"),"Add a chapter using the + Chapter button above." },
        { ("en", "editor.no.questions"),   "No questions in this chapter yet." },
        { ("en", "editor.no.questions.hint"),"Add the first question using the button above." },
        { ("en", "editor.new.question"),   "New Question" },
        { ("en", "editor.edit.question"),  "Edit Question" },
        { ("en", "editor.question.text"),  "Question Text" },
        { ("en", "editor.question.text.ph"),"Enter the question..." },
        { ("en", "editor.points"),         "Points (1–100)" },
        { ("en", "editor.correct.answer"), "Correct Answer" },
        { ("en", "editor.rename"),         "Rename" },
        { ("en", "editor.delete.chapter.title"), "Delete chapter" },

        { ("ar", "editor.add.question"),   "+ إضافة سؤال" },
        { ("ar", "editor.add.chapter"),    "+ فصل" },
        { ("ar", "editor.chapter.ph"),     "اسم الفصل..." },
        { ("ar", "editor.select.chapter"), "اختر فصلاً لعرض الأسئلة وإدارتها." },
        { ("ar", "editor.no.chapters"),    "لا توجد فصول بعد." },
        { ("ar", "editor.no.chapters.hint"),"أضف فصلاً باستخدام زر + فصل أعلاه." },
        { ("ar", "editor.no.questions"),   "لا توجد أسئلة في هذا الفصل بعد." },
        { ("ar", "editor.no.questions.hint"),"أضف السؤال الأول باستخدام الزر أعلاه." },
        { ("ar", "editor.new.question"),   "سؤال جديد" },
        { ("ar", "editor.edit.question"),  "تعديل سؤال" },
        { ("ar", "editor.question.text"),  "نص السؤال" },
        { ("ar", "editor.question.text.ph"),"أدخل السؤال..." },
        { ("ar", "editor.points"),         "النقاط (1–100)" },
        { ("ar", "editor.correct.answer"), "الإجابة الصحيحة" },
        { ("ar", "editor.rename"),         "إعادة التسمية" },
        { ("ar", "editor.delete.chapter.title"), "حذف الفصل" },

        // --- Instructor courses page ---
        { ("en", "courses.no.courses"),    "No courses found." },
        { ("en", "courses.no.courses.hint"),"Courses are managed by administrators. Contact your admin." },
        { ("en", "courses.nav.banks"),     "Banks" },
        { ("en", "courses.nav.exams"),     "Exams" },
        { ("en", "courses.nav.sections"),  "Sections" },

        { ("ar", "courses.no.courses"),    "لا توجد مقررات." },
        { ("ar", "courses.no.courses.hint"),"تُدار المقررات من قِبل المسؤولين. تواصل مع المسؤول." },
        { ("ar", "courses.nav.banks"),     "البنوك" },
        { ("ar", "courses.nav.exams"),     "الاختبارات" },
        { ("ar", "courses.nav.sections"),  "الشعب" },

        // --- Admin Users ---
        { ("en", "users.form.create"),     "Create New User" },
        { ("en", "users.fullname.ph"),     "Full Name" },
        { ("en", "users.email.ph"),        "email@university.edu" },
        { ("en", "users.password.ph"),     "Min 8 characters" },
        { ("en", "users.search.ph"),       "Search by name or email..." },
        { ("en", "users.select.role"),     "Select..." },
        { ("en", "users.add.role"),        "Add" },
        { ("en", "users.add.role.btn"),    "+ Role" },
        { ("en", "users.err.required"),    "Please fill in all required fields." },
        { ("en", "users.err.password"),    "Password must be at least 8 characters." },

        { ("ar", "users.form.create"),     "إنشاء مستخدم جديد" },
        { ("ar", "users.fullname.ph"),     "الاسم الكامل" },
        { ("ar", "users.email.ph"),        "email@university.edu" },
        { ("ar", "users.password.ph"),     "8 أحرف على الأقل" },
        { ("ar", "users.search.ph"),       "ابحث بالاسم أو البريد الإلكتروني..." },
        { ("ar", "users.select.role"),     "اختر..." },
        { ("ar", "users.add.role"),        "إضافة" },
        { ("ar", "users.add.role.btn"),    "+ دور" },
        { ("ar", "users.err.required"),    "يرجى ملء جميع الحقول المطلوبة." },
        { ("ar", "users.err.password"),    "يجب أن تكون كلمة المرور 8 أحرف على الأقل." },

        // --- Admin Courses ---
        { ("en", "admin.courses.form.new"),  "New Course" },
        { ("en", "admin.courses.name.ph"),   "e.g. Data Structures" },
        { ("en", "admin.courses.code.hint"), "Letters followed by numbers, e.g. CPCS202" },
        { ("en", "admin.courses.err.fill"),  "Please provide both name and code." },
        { ("en", "admin.courses.err.code"),  "Code must be letters followed by numbers (e.g. CPCS202)." },

        { ("ar", "admin.courses.form.new"),  "مقرر جديد" },
        { ("ar", "admin.courses.name.ph"),   "مثال: هياكل البيانات" },
        { ("ar", "admin.courses.code.hint"), "حروف تليها أرقام، مثال: CPCS202" },
        { ("ar", "admin.courses.err.fill"),  "يرجى تقديم الاسم والرمز." },
        { ("ar", "admin.courses.err.code"),  "يجب أن يتكون الرمز من حروف تليها أرقام (مثال: CPCS202)." },

        // --- Admin Dashboard ---
        { ("en", "admin.loading"),           "Loading dashboard..." },
        { ("en", "admin.allusers"),          "All Users" },
        { ("en", "admin.users.manage.hint"), "Create, manage and assign roles" },
        { ("en", "admin.courses.manage.hint"),"Add, edit and remove courses" },
        { ("en", "admin.stat.totalusers"),   "Total Users" },
        { ("en", "admin.stat.instructors"),  "Instructors" },
        { ("en", "admin.stat.students"),     "Students" },
        { ("en", "admin.stat.courses"),      "Courses" },
        { ("en", "admin.stat.registered"),   "registered" },
        { ("en", "admin.stat.active"),       "active" },
        { ("en", "admin.stat.semester"),     "this semester" },

        { ("ar", "admin.loading"),           "جارٍ تحميل لوحة التحكم..." },
        { ("ar", "admin.allusers"),          "جميع المستخدمين" },
        { ("ar", "admin.users.manage.hint"), "إنشاء المستخدمين وإدارتهم وتعيين الأدوار" },
        { ("ar", "admin.courses.manage.hint"),"إضافة المقررات وتعديلها وحذفها" },
        { ("ar", "admin.stat.totalusers"),   "إجمالي المستخدمين" },
        { ("ar", "admin.stat.instructors"),  "المحاضرون" },
        { ("ar", "admin.stat.students"),     "الطلاب" },
        { ("ar", "admin.stat.courses"),      "المقررات" },
        { ("ar", "admin.stat.registered"),   "مسجّل" },
        { ("ar", "admin.stat.active"),       "نشط" },
        { ("ar", "admin.stat.semester"),     "هذا الفصل" },

        // --- Exam validation ---
        { ("en", "exam.err.title"),          "Title is required." },
        { ("en", "exam.err.endtime"),        "End time must be after start time." },
        { ("en", "exam.err.passscore"),      "Passing score cannot exceed max score." },

        { ("ar", "exam.err.title"),          "العنوان مطلوب." },
        { ("ar", "exam.err.endtime"),        "يجب أن يكون وقت الانتهاء بعد وقت البدء." },
        { ("ar", "exam.err.passscore"),      "لا يمكن أن تتجاوز درجة النجاح الدرجة القصوى." },
    };
}
