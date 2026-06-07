namespace PracticalWork.Reports.Worker.Abstractions;

/// <summary>
/// Сервис полного цикла генерации и публикации отчета.
/// </summary>
public interface IReportGenerationService
{
    /// <summary>
    /// Генерирует отчет, сохраняет его и обновляет статус.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <param name="periodFrom">Дата начала периода.</param>
    /// <param name="periodTo">Дата окончания периода.</param>
    /// <param name="eventTypes">Типы событий, включаемые в отчет.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task GenerateReportAsync(
        Guid reportId,
        DateOnly? periodFrom,
        DateOnly? periodTo,
        string[] eventTypes,
        CancellationToken cancellationToken);
}
