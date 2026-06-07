namespace PracticalWork.Library.Contracts.v1.Reports.Request;

/// <summary>
/// Создание отчета
/// </summary>
/// <param name="PeriodFrom">Фильтр на дату начала</param>
/// <param name="PeriodTo">Фильтр на дату окончания</param>
/// <param name="EventTypes">Фильтр на типы событий</param>
public record ReportCreateRequest(
    DateOnly PeriodFrom, 
    DateOnly PeriodTo, 
    IReadOnlyList<string> EventTypes);