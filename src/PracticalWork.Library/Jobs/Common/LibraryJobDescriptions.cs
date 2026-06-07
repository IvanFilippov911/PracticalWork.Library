namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Описания фоновых задач.
/// </summary>
public static class LibraryJobDescriptions
{
    private static readonly Dictionary<string, string> Descriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        [LibraryJobNames.ReturnReminder] = "Ежедневная отправка напоминаний о возврате книг.",
        [LibraryJobNames.WeeklyAdminReport] = "Еженедельный отчет администрации и выгрузка CSV в MinIO.",
        [LibraryJobNames.ArchiveOldBooks] = "Ежемесячная архивация старых и невыдаваемых книг."
    };

    public static string GetDescription(string jobName) =>
        Descriptions.TryGetValue(jobName, out var description)
            ? description
            : "Описание не задано.";
}
