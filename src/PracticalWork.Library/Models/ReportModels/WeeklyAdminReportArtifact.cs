namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Артефакт опубликованного weekly report.
/// </summary>
public sealed class WeeklyAdminReportArtifact
{
    /// <summary>
    /// Имя отчета.
    /// </summary>
    public required string ReportName { get; init; }

    /// <summary>
    /// Ссылка на отчет.
    /// </summary>
    public required string ReportUrl { get; init; }

    /// <summary>
    /// Bucket отчета.
    /// </summary>
    public required string BucketName { get; init; }

    /// <summary>
    /// Object name отчета.
    /// </summary>
    public required string ObjectName { get; init; }
}
