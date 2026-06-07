using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сборщик email для еженедельного административного отчета.
/// </summary>
public interface IWeeklyAdminReportEmailBuilder
{
    /// <summary>
    /// Собрать email-сообщение для администратора.
    /// </summary>
    Task<EmailMessage> BuildAsync(
        string adminEmail,
        DateOnly periodFrom,
        DateOnly periodTo,
        WeeklyAdminReportStatistics statistics,
        string reportUrl,
        WeeklyReportTemplate template,
        CancellationToken cancellationToken = default);
}
