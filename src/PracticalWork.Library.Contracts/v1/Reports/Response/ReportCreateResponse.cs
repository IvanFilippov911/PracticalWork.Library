using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Reports.Response;

/// <summary>
/// Ответ после создания отчета.
/// </summary>
public class ReportCreateResponse
{
    public DateOnly? PeriodFrom { get; set; }

    public DateOnly? PeriodTo { get; set; }

    public IReadOnlyList<string> EventTypes { get; set; }

    public ReportStatus Status { get; set; }
}
