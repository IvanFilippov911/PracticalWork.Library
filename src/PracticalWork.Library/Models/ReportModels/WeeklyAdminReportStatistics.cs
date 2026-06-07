namespace PracticalWork.Library.Models.ReportModels;

/// <summary>
/// Статистика еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportStatistics
{
    public int NewBooksCount { get; init; }
    public int NewReadersCount { get; init; }
    public int BorrowedCount { get; init; }
    public int ReturnedCount { get; init; }
    public int OverdueCount { get; init; }
}
