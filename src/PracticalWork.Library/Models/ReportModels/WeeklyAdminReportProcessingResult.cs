namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Результат подготовки и рассылки еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportProcessingResult
{
    /// <summary>
    /// Имя сформированного отчета.
    /// </summary>
    public required string ReportName { get; init; }

    /// <summary>
    /// Количество получателей.
    /// </summary>
    public int AdminEmailCount { get; init; }

    /// <summary>
    /// Количество успешных отправок.
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Количество неудачных отправок.
    /// </summary>
    public int FailureCount { get; init; }
}
