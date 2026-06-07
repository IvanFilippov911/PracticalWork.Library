using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Helpers;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Reports.Request;
using PracticalWork.Library.Contracts.v1.Reports.Response;

namespace PracticalWork.Reports.Web.Services;

/// <summary>
/// Сервис сценариев чтения журнала активности и управления отчетами.
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IMessageProducer _producer;
    private readonly IMinioService _minioService;
    private readonly ICacheService _cacheService;
    private readonly ICacheVersionService _cacheVersionService;
    private readonly MinioOptions _minioOptions;
    private readonly BooksCacheOptions _cacheOptions;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Инициализирует сервис отчетов.
    /// </summary>
    /// <param name="activityLogRepository">Репозиторий журнала активности.</param>
    /// <param name="reportRepository">Репозиторий отчетов.</param>
    /// <param name="producer">Продюсер сообщений для постановки генерации отчета.</param>
    /// <param name="cacheService">Сервис распределенного кэша.</param>
    /// <param name="minioService">Сервис файлового хранилища.</param>
    /// <param name="minioOptions">Опции MinIO.</param>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="cacheOptions">Опции кэша.</param>
    /// <param name="timeProvider">Поставщик времени.</param>
    public ReportService(
        IActivityLogRepository activityLogRepository,
        IReportRepository reportRepository,
        IMessageProducer producer,
        ICacheService cacheService,
        IMinioService minioService,
        IOptionsMonitor<MinioOptions> minioOptions,
        ICacheVersionService cacheVersionService,
        IOptionsMonitor<BooksCacheOptions> cacheOptions,
        TimeProvider timeProvider)
    {
        _activityLogRepository = activityLogRepository;
        _reportRepository = reportRepository;
        _cacheService = cacheService;
        _producer = producer;
        _minioService = minioService;
        _minioOptions = minioOptions.CurrentValue;
        _cacheVersionService = cacheVersionService;
        _cacheOptions = cacheOptions.CurrentValue;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Возвращает страницу записей о событиях системы.
    /// </summary>
    /// <param name="request">Параметры пагинации и фильтрации журнала.</param>
    /// <returns>Страница журнала активности.</returns>
    public async Task<PaginationResponse<ActivityLogResponse>> ReadSystemActivityLogs(ActivityLogsPaginationRequest request)
    {
        var logs = await _activityLogRepository.GetLogsPageAsync(request);

        return new PaginationResponse<ActivityLogResponse>
        {
            Entities = logs.Item1
                .Select(log => new ActivityLogResponse(log.Event, log.EventType, log.EventDate))
                .ToList(),
            TotalCount = logs.Item2,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };
    }

    /// <summary>
    /// Создает запись о новом отчете и публикует событие на его фоновую генерацию.
    /// </summary>
    /// <param name="request">Параметры построения отчета.</param>
    /// <returns>Информация о созданном отчете.</returns>
    public async Task<ReportCreateResponse> CreateReport(ReportCreateRequest request)
    {
        var report = new Report
        {
            PeriodFrom = request.PeriodFrom,
            PeriodTo = request.PeriodTo,
            EventTypes = request.EventTypes.ToArray(),
        };

        var id = await _reportRepository.CreateReport(report);
        var message = new ReportCreateEvent(id, request.PeriodFrom, request.PeriodTo, request.EventTypes, report.Status);
        await _producer.ProduceReportCreateAsync(message);
        await CacheManager.InvalidateReportsCacheAsync(_cacheVersionService, _cacheOptions);

        return new ReportCreateResponse
        {
            EventTypes = report.EventTypes,
            PeriodFrom = report.PeriodFrom,
            PeriodTo = report.PeriodTo,
            Status = report.Status,
        };
    }

    /// <summary>
    /// Возвращает список отчетов в готовом состоянии.
    /// </summary>
    /// <returns>Коллекция готовых отчетов.</returns>
    public async Task<IReadOnlyList<ReportResponse>> GetListOfReadyReports()
    {
        var cacheCheckResult = await CacheManager.CheckCacheAsync<ReportResponse, ReportResponse>(
            _cacheVersionService,
            _cacheService,
            _cacheOptions.ReportsCacheOptions.Prefix,
            "ready",
            dto => dto);

        if (cacheCheckResult.Count != 0)
        {
            return cacheCheckResult;
        }

        var reports = await _reportRepository.GetReadyReports();

        var response = reports
            .Select(report => new ReportResponse(
                report.Name,
                report.FilePath,
                report.PeriodFrom,
                report.PeriodTo,
                report.EventTypes,
                report.GeneratedAt))
            .ToList();

        await CacheManager.WriteToCacheAsync(
            _cacheVersionService,
            _cacheService,
            _cacheOptions.ReportsCacheOptions,
            "ready",
            response,
            model => model);

        return response;
    }

    /// <summary>
    /// Возвращает ссылку на скачивание файла отчета.
    /// </summary>
    /// <param name="reportName">Имя отчета.</param>
    /// <returns>Ссылка на скачивание файла.</returns>
    public async Task<string> GetReportUrl(string reportName)
    {
        var (id, report) = await _reportRepository.GetReportByName(reportName);
        var generatedDate = report.GeneratedAt ?? _timeProvider.GetUtcNow().UtcDateTime;
        var fileName = $"{generatedDate.Year}/{generatedDate.Month}/{reportName}";
        var filePath = await _minioService.GetFileUrlAsync(_minioOptions.ReportsBucketName, fileName);
        report.FilePath = filePath;
        await _reportRepository.UpdateReport(id, report);
        await CacheManager.InvalidateReportsCacheAsync(_cacheVersionService, _cacheOptions);
        return filePath;
    }
}
