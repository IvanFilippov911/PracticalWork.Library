using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Helpers;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.Contracts.Options;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Reports.Worker.Abstractions;
using PracticalWork.Reports.Worker.Models;

namespace PracticalWork.Reports.Worker.Services;

/// <summary>
/// Сервис orchestration генерации отчета: чтение данных, формирование файла и обновление статуса.
/// </summary>
public sealed class ReportGenerationService : IReportGenerationService
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IMinioService _minioService;
    private readonly IReportGenerateService _reportGenerateService;
    private readonly ICacheVersionService _cacheVersionService;
    private readonly MinioOptions _minioOptions;
    private readonly BooksCacheOptions _cacheOptions;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Инициализирует сервис генерации отчета.
    /// </summary>
    /// <param name="activityLogRepository">Репозиторий журнала активности.</param>
    /// <param name="reportRepository">Репозиторий отчетов.</param>
    /// <param name="minioService">Сервис доступа к файловому хранилищу.</param>
    /// <param name="reportGenerateService">Генератор содержимого файла отчета.</param>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="minioOptions">Опции MinIO.</param>
    /// <param name="cacheOptions">Опции кэша.</param>
    /// <param name="timeProvider">Поставщик времени.</param>
    public ReportGenerationService(
        IActivityLogRepository activityLogRepository,
        IReportRepository reportRepository,
        IMinioService minioService,
        IReportGenerateService reportGenerateService,
        ICacheVersionService cacheVersionService,
        IOptionsMonitor<MinioOptions> minioOptions,
        IOptionsMonitor<BooksCacheOptions> cacheOptions,
        TimeProvider timeProvider)
    {
        _activityLogRepository = activityLogRepository;
        _reportRepository = reportRepository;
        _minioService = minioService;
        _reportGenerateService = reportGenerateService;
        _cacheVersionService = cacheVersionService;
        _minioOptions = minioOptions.CurrentValue;
        _cacheOptions = cacheOptions.CurrentValue;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Генерирует отчет, загружает его в MinIO и обновляет состояние в базе данных.
    /// </summary>
    /// <param name="reportId">Идентификатор отчета.</param>
    /// <param name="periodFrom">Дата начала периода.</param>
    /// <param name="periodTo">Дата окончания периода.</param>
    /// <param name="eventTypes">Типы событий, включаемые в отчет.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task GenerateReportAsync(
        Guid reportId,
        DateOnly? periodFrom,
        DateOnly? periodTo,
        string[] eventTypes,
        CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetReportById(reportId);
        var logs = await _activityLogRepository.GetLogsAsync(periodFrom, periodTo, eventTypes);

        try
        {
            var reportResult = _reportGenerateService.GenerateReport(reportId, logs.Item1);

            await _minioService.UploadFileAsync(
                _minioOptions.ReportsBucketName,
                reportResult.FileName,
                reportResult.Content,
                reportResult.ContentType);

            var fileName = reportResult.FileName.Split('/')[^1];
            report.MarkAsGenerated(fileName, _timeProvider);
            await _reportRepository.UpdateReport(reportId, report);
            await CacheManager.InvalidateReportsCacheAsync(_cacheVersionService, _cacheOptions);
        }
        catch (Exception)
        {
            report.Status = ReportStatus.Error;
            await _reportRepository.UpdateReport(reportId, report);
            await CacheManager.InvalidateReportsCacheAsync(_cacheVersionService, _cacheOptions);
            throw;
        }
    }
}
