using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Reports.Worker.Models;

namespace PracticalWork.Reports.Worker.Abstractions;

/// <summary>
/// Генерирует файл отчета по журналу активности.
/// </summary>
public interface IReportGenerateService
{
    /// <summary>
    /// Формирует файл отчета на основе переданных записей журнала.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <param name="logs">Записи журнала активности.</param>
    /// <returns>Результат генерации файла отчета.</returns>
    ReportGenerateResult GenerateReport(Guid reportId, IReadOnlyList<ActivityLog> logs);
}
