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
        { ("en", "nav.exams"),      "Exams" },
        { ("en", "nav.instructor"), "Instructor" },
        { ("en", "nav.admin"),      "Admin" },

        { ("ar", "nav.login"),      "تسجيل الدخول" },
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

        { ("ar", "login.welcome"),       "مرحباً بعودتك" },
        { ("ar", "login.subtitle"),      "سجّل دخولك إلى نظام الاختبارات الإلكترونية" },
        { ("ar", "login.email"),         "البريد الإلكتروني" },
        { ("ar", "login.email.ph"),      "you@university.edu" },
        { ("ar", "login.password"),      "كلمة المرور" },
        { ("ar", "login.submit"),        "تسجيل الدخول" },
        { ("ar", "login.loading"),       "جارٍ تسجيل الدخول..." },
        { ("ar", "login.err.empty"),     "يرجى إدخال البريد الإلكتروني وكلمة المرور." },
        { ("ar", "login.err.invalid"),   "بيانات غير صحيحة." },

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
    };
}
