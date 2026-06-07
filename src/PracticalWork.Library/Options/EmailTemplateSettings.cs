namespace PracticalWork.Library.Options;

/// <summary>
/// Настройки email шаблонов.
/// </summary>
public class EmailTemplateSettings
{
    public ReturnReminderTemplate ReturnReminder { get; set; } = new();

    public WeeklyReportTemplate WeeklyReport { get; set; } = new();
}

/// <summary>
/// Шаблон для напоминания о возврате книги.
/// </summary>
public class ReturnReminderTemplate
{
    public string SubjectTemplate { get; set; } = "Напоминание о возврате книги: \"{BookTitle}\"";

    public int DaysBeforeDueDate { get; set; } = 3;

    public string LibraryName { get; set; } = "Библиотека";

    public string LibraryAddress { get; set; } = string.Empty;

    public string LibraryPhone { get; set; } = string.Empty;

    public string WorkingHours { get; set; } = string.Empty;
}

/// <summary>
/// Шаблон для еженедельного отчета администрации.
/// </summary>
public class WeeklyReportTemplate
{
    public string SubjectTemplate { get; set; } =
        "Еженедельный отчет библиотеки за период {StartDate} - {EndDate}";

    public string[] AdminEmails { get; set; } = Array.Empty<string>();

    public int ReportRetentionDays { get; set; } = 90;

    public string ReportsBucketName { get; set; } = "library-reports";
}
