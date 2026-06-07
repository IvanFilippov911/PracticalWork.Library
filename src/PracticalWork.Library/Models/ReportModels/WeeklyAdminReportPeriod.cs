namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Период и временная зона weekly report.
/// </summary>
public sealed class WeeklyAdminReportPeriod
{
    /// <summary>
    /// Начало периода.
    /// </summary>
    public required DateOnly PeriodFrom { get; init; }

    /// <summary>
    /// Конец периода.
    /// </summary>
    public required DateOnly PeriodTo { get; init; }

    /// <summary>
    /// Временная зона расчета.
    /// </summary>
    public required TimeZoneInfo TimeZone { get; init; }
}
