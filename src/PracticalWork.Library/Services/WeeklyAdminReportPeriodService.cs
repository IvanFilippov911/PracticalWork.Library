using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Расчет периода weekly report.
/// </summary>
public sealed class WeeklyAdminReportPeriodService : IWeeklyAdminReportPeriodService
{
    private readonly TimeProvider _timeProvider;

    public WeeklyAdminReportPeriodService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public WeeklyAdminReportPeriod GetPreviousWeek(string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().UtcDateTime, timeZone).Date;
        var daysSinceMonday = ((int)nowLocal.DayOfWeek + 6) % 7;

        var currentWeekMonday = nowLocal.AddDays(-daysSinceMonday);
        var previousWeekMonday = currentWeekMonday.AddDays(-7);
        var previousWeekSunday = currentWeekMonday.AddDays(-1);

        return new WeeklyAdminReportPeriod
        {
            PeriodFrom = DateOnly.FromDateTime(previousWeekMonday),
            PeriodTo = DateOnly.FromDateTime(previousWeekSunday),
            TimeZone = timeZone
        };
    }
}
