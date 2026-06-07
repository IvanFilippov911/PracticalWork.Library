using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис публикации и очистки weekly report файлов.
/// </summary>
public interface IWeeklyAdminReportStorageService
{
    /// <summary>
    /// Сформировать, сохранить и зарегистрировать weekly report.
    /// </summary>
    Task<WeeklyAdminReportArtifact> PublishAsync(
        WeeklyAdminReportProcessingRequest request,
        WeeklyAdminReportPeriod period,
        WeeklyAdminReportStatistics statistics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистить старые weekly report файлы.
    /// </summary>
    Task CleanupAsync(
        string bucket,
        int retentionDays,
        CancellationToken cancellationToken = default);
}
