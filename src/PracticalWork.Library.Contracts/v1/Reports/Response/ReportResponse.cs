namespace PracticalWork.Library.Contracts.v1.Reports.Response;

/// <summary>
/// Отчет
/// </summary>
/// <param name="ReportName">Название файла отчета</param>
/// <param name="FilePath">Ссылка на файл отчета</param>
/// <param name="PeriodFrom">Фильтр даты начала</param>
/// <param name="PeriodTo">Фильтр даты окончания</param>
/// <param name="EventTypes">Фильтр типов событий</param>
/// <param name="GeneratedAt">Когда создан</param>
public record ReportResponse(
    string ReportName, 
    string FilePath,
    DateOnly? PeriodFrom, 
    DateOnly? PeriodTo, 
    IReadOnlyList<string> EventTypes,
    DateTime? GeneratedAt);