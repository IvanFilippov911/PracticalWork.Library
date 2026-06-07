using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Library.Contracts.v1.Reports.Response;

namespace PracticalWork.Library.Contracts.Abstractions.Services;

/// <summary>
/// Контракт сервиса отчетов.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Прочитать страницу с записями событий системы.
    /// </summary>
    Task<PaginationResponse<ActivityLogResponse>> ReadSystemActivityLogs(ActivityLogsPaginationRequest request);

    /// <summary>
    /// Создать отчет по событиям системы.
    /// </summary>
    Task<ReportCreateResponse> CreateReport(ReportCreateRequest request);

    /// <summary>
    /// Получить список готовых отчетов.
    /// </summary>
    Task<IReadOnlyList<ReportResponse>> GetListOfReadyReports();

    /// <summary>
    /// Получить ссылку на отчет.
    /// </summary>
    Task<string> GetReportUrl(string reportName);
}
