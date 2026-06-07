using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.Models.ReportModels;

/// <summary>
/// Отчет с записями событий системы.
/// </summary>
public class Report
{
    public string Name { get; set; }

    public string FilePath { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public DateOnly? PeriodFrom { get; set; }

    public DateOnly? PeriodTo { get; set; }

    public IReadOnlyList<string> EventTypes { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.InProgress;

    public void MarkAsGenerated(string fileName, TimeProvider timeProvider)
    {
        GeneratedAt = timeProvider.GetUtcNow().UtcDateTime;
        Status = ReportStatus.Generated;
        Name = fileName;
    }
}
