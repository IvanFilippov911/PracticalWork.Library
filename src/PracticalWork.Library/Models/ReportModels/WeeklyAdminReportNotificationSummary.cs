namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Результат рассылки weekly report.
/// </summary>
public sealed class WeeklyAdminReportNotificationSummary
{
    /// <summary>
    /// Количество получателей.
    /// </summary>
    public int AdminEmailCount { get; init; }

    /// <summary>
    /// Количество успешных отправок.
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Количество неуспешных отправок.
    /// </summary>
    public int FailureCount { get; init; }
}
