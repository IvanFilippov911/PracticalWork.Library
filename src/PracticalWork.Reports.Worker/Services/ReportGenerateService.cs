using System.Text;
using System.Text.Json;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Reports.Worker.Abstractions;
using PracticalWork.Reports.Worker.Models;

namespace PracticalWork.Reports.Worker.Services;

/// <summary>
/// Сервис генерации CSV-файла отчета по журналу активности.
/// </summary>
public sealed class ReportGenerateService : IReportGenerateService
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Инициализирует генератор файла отчета.
    /// </summary>
    /// <param name="timeProvider">Поставщик времени.</param>
    public ReportGenerateService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Формирует содержимое CSV-отчета.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <param name="logs">Записи журнала активности.</param>
    /// <returns>Результат генерации файла.</returns>
    public ReportGenerateResult GenerateReport(Guid reportId, IReadOnlyList<ActivityLog> logs)
    {
        var timestamp = _timeProvider.GetUtcNow().UtcDateTime;
        var fileName = $"{timestamp.Year}/{timestamp.Month}/{reportId}.csv";
        const string contentType = "text/csv";

        var sb = new StringBuilder();
        sb.AppendLine("EventType;EventDate;Metadata");

        foreach (var log in logs)
        {
            sb.AppendLine($"{log.EventType};{log.EventDate};{JsonSerializer.Serialize(log, log.GetType())}");
        }

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));

        return new ReportGenerateResult
        {
            FileName = fileName,
            Content = stream,
            ContentType = contentType
        };
    }
}
