using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис рассылки weekly report уведомлений.
/// </summary>
public interface IWeeklyAdminReportNotificationService
{
    /// <summary>
    /// Отправить weekly report администраторам.
    /// </summary>
    Task<WeeklyAdminReportNotificationSummary> SendAsync(
        WeeklyAdminReportProcessingRequest request,
        WeeklyAdminReportPeriod period,
        WeeklyAdminReportStatistics statistics,
        string reportUrl,
        CancellationToken cancellationToken = default);
}
