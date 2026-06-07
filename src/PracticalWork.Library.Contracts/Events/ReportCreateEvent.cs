using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие постановки отчета в генерацию.
/// </summary>
/// <param name="Id">Идентификатор отчета.</param>
/// <param name="PeriodFrom">Дата начала периода.</param>
/// <param name="PeriodTo">Дата окончания периода.</param>
/// <param name="EventTypes">Типы событий, включаемые в отчет.</param>
/// <param name="Status">Текущий статус отчета.</param>
public sealed record ReportCreateEvent(
    Guid Id,
    DateOnly? PeriodFrom,
    DateOnly? PeriodTo,
    IReadOnlyList<string> EventTypes,
    ReportStatus Status
) : BaseEvent(Guid.NewGuid(), "report.create", "report-service");
