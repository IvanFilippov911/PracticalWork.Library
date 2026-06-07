using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сборщик CSV для еженедельного административного отчета.
/// </summary>
public interface IWeeklyAdminReportCsvBuilder
{
    /// <summary>
    /// Построить CSV-содержимое отчета.
    /// </summary>
    string Build(
        DateOnly periodFrom,
        DateOnly periodTo,
        WeeklyAdminReportStatistics statistics);
}
