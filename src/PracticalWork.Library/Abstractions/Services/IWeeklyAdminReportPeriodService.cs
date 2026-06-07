using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис расчета периода weekly report.
/// </summary>
public interface IWeeklyAdminReportPeriodService
{
    /// <summary>
    /// Рассчитать предыдущую полную неделю для указанной временной зоны.
    /// </summary>
    WeeklyAdminReportPeriod GetPreviousWeek(string timeZoneId);
}
