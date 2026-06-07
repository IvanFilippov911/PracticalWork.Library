namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Метаданные еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportMetadata
{
    /// <summary>
    /// Имя отчета.
    /// </summary>
    public required string ReportName { get; init; }

    /// <summary>
    /// Имя bucket.
    /// </summary>
    public required string BucketName { get; init; }

    /// <summary>
    /// Имя объекта в bucket.
    /// </summary>
    public required string ObjectName { get; init; }

    /// <summary>
    /// Начало периода.
    /// </summary>
    public DateOnly PeriodFrom { get; init; }

    /// <summary>
    /// Конец периода.
    /// </summary>
    public DateOnly PeriodTo { get; init; }

    /// <summary>
    /// Статистика отчета.
    /// </summary>
    public required WeeklyAdminReportStatistics Statistics { get; init; }
}
