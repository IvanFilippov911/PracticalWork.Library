namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Имена фоновых задач библиотеки.
/// </summary>
public static class LibraryJobNames
{
    public const string ReturnReminder = "ReturnReminderJob";
    public const string WeeklyAdminReport = "WeeklyAdminReportJob";
    public const string ArchiveOldBooks = "ArchiveOldBooksJob";

    public static readonly string[] All =
    [
        ReturnReminder,
        WeeklyAdminReport,
        ArchiveOldBooks
    ];
}
