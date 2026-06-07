using PracticalWork.Library.Contracts.Models.ReportModels;

namespace PracticalWork.Library.Contracts.Abstractions.Storage;

/// <summary>
/// Репозиторий получения данных об отчетах.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Создает новый отчет.
    /// </summary>
    /// <param name="report">Модель создаваемого отчета.</param>
    /// <returns>Идентификатор созданного отчета.</returns>
    Task<Guid> CreateReport(Report report);

    /// <summary>
    /// Возвращает список готовых отчетов.
    /// </summary>
    /// <returns>Коллекция отчетов в статусе готовности.</returns>
    Task<IReadOnlyList<Report>> GetReadyReports();

    /// <summary>
    /// Возвращает отчет по идентификатору.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <returns>Найденный отчет.</returns>
    Task<Report> GetReportById(Guid reportId);

    /// <summary>
    /// Возвращает отчет по имени.
    /// </summary>
    /// <param name="reportName">Имя отчета.</param>
    /// <returns>Идентификатор и модель найденного отчета.</returns>
    Task<(Guid id, Report report)> GetReportByName(string reportName);

    /// <summary>
    /// Обновляет состояние или содержимое отчета.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <param name="report">Новая модель отчета.</param>
    Task UpdateReport(Guid reportId, Report report);
}
