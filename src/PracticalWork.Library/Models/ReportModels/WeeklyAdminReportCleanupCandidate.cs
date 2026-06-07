namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Старый weekly report, подлежащий очистке.
/// </summary>
public sealed class WeeklyAdminReportCleanupCandidate
{
    /// <summary>
    /// Идентификатор записи метаданных.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Имя отчета.
    /// </summary>
    public required string ReportName { get; init; }

    /// <summary>
    /// Имя объекта в bucket.
    /// </summary>
    public required string ObjectName { get; init; }
}
