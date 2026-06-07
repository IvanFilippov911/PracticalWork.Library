using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Reports.Request;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для работы с журналом активности и отчетами.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/reports")]
public sealed class ReportController : Controller
{
    private readonly IReportService _reportService;

    /// <summary>
    /// Инициализирует контроллер отчетов.
    /// </summary>
    /// <param name="reportService">Сервис сценариев отчетов.</param>
    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Получение страницы записей о событиях системы
    /// </summary>
    /// <param name="request">Параметры пагинации и фильтрации журнала событий.</param>
    [HttpPost("activity")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActivityLogs(ActivityLogsPaginationRequest request)
    {
        return Ok(await _reportService.ReadSystemActivityLogs(request));
    }

    /// <summary>
    /// Создать отчет csv
    /// </summary>
    /// <param name="request">Параметры построения отчета.</param>
    [HttpPost("generate")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReportCsv(ReportCreateRequest request)
    {
        return Ok(await _reportService.CreateReport(request));
    }

    /// <summary>
    /// Получить созданные отчеты
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGeneratedReports()
    {
        return Ok(await _reportService.GetListOfReadyReports());
    }

    /// <summary>
    /// Получение ссылки на файл отчета
    /// </summary>
    /// <param name="reportName">Имя готового отчета.</param>
    [HttpGet("{reportName}/download")]
    [Produces("text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGeneratedReportUrl(string reportName)
    {
        var result = await _reportService.GetReportUrl(reportName);
        return Ok(result);
    }
}
