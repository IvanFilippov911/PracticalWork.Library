using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис сценария подготовки и рассылки еженедельного административного отчета.
/// </summary>
public interface IWeeklyAdminReportProcessingService
{
    /// <summary>
    /// Выполнить подготовку и рассылку отчета.
    /// </summary>
    Task<WeeklyAdminReportProcessingResult> ProcessAsync(
        WeeklyAdminReportProcessingRequest request,
        CancellationToken cancellationToken = default);
}
