using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Репозиторий данных для еженедельного административного отчета.
/// </summary>
public interface IWeeklyAdminReportRepository
{
    /// <summary>
    /// Получить статистику за период.
    /// </summary>
    Task<WeeklyAdminReportStatistics> GetStatistics(
        DateOnly periodFrom,
        DateOnly periodTo,
        TimeZoneInfo timeZone,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать или обновить метаданные еженедельного отчета.
    /// </summary>
    Task UpsertMetadata(
        WeeklyAdminReportMetadata metadata,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить старые отчеты для очистки.
    /// </summary>
    Task<IReadOnlyList<WeeklyAdminReportCleanupCandidate>> GetReportsForCleanup(
        DateTime cutoffUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отметить отчеты удаленными.
    /// </summary>
    Task MarkReportsDeleted(
        IReadOnlyCollection<Guid> reportIds,
        DateTime deletedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить логи отправки уведомлений по weekly report.
    /// </summary>
    Task SaveNotificationLogs(
        IReadOnlyCollection<WeeklyAdminReportNotificationLogEntry> logs,
        CancellationToken cancellationToken = default);
}
