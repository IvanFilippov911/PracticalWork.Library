using System.Text;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Публикация и очистка weekly report файлов.
/// </summary>
public sealed class WeeklyAdminReportStorageService : IWeeklyAdminReportStorageService
{
    private readonly IWeeklyAdminReportRepository _weeklyAdminReportRepository;
    private readonly IWeeklyAdminReportCsvBuilder _weeklyAdminReportCsvBuilder;
    private readonly IMinioService _minioService;
    private readonly TimeProvider _timeProvider;

    public WeeklyAdminReportStorageService(
        IWeeklyAdminReportRepository weeklyAdminReportRepository,
        IWeeklyAdminReportCsvBuilder weeklyAdminReportCsvBuilder,
        IMinioService minioService,
        TimeProvider timeProvider)
    {
        _weeklyAdminReportRepository = weeklyAdminReportRepository;
        _weeklyAdminReportCsvBuilder = weeklyAdminReportCsvBuilder;
        _minioService = minioService;
        _timeProvider = timeProvider;
    }

    public async Task<WeeklyAdminReportArtifact> PublishAsync(
        WeeklyAdminReportProcessingRequest request,
        WeeklyAdminReportPeriod period,
        WeeklyAdminReportStatistics statistics,
        CancellationToken cancellationToken = default)
    {
        var reportName = $"report_{period.PeriodTo:yyyy-MM-dd}.csv";
        var bucket = request.Template.ReportsBucketName;
        var csvContent = _weeklyAdminReportCsvBuilder.Build(period.PeriodFrom, period.PeriodTo, statistics);

        await using (var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
        {
            await _minioService.UploadFileAsync(bucket, reportName, csvStream, "text/csv", cancellationToken);
        }

        var reportUrl = await _minioService.GetFileUrlAsync(bucket, reportName, cancellationToken);

        await _weeklyAdminReportRepository.UpsertMetadata(
            new WeeklyAdminReportMetadata
            {
                ReportName = reportName,
                BucketName = bucket,
                ObjectName = reportName,
                PeriodFrom = period.PeriodFrom,
                PeriodTo = period.PeriodTo,
                Statistics = statistics
            },
            cancellationToken);

        return new WeeklyAdminReportArtifact
        {
            ReportName = reportName,
            ReportUrl = reportUrl,
            BucketName = bucket,
            ObjectName = reportName
        };
    }

    public async Task CleanupAsync(
        string bucket,
        int retentionDays,
        CancellationToken cancellationToken = default)
    {
        var cutoffUtc = _timeProvider.GetUtcNow().UtcDateTime.AddDays(-Math.Max(1, retentionDays));
        var oldReports = await _weeklyAdminReportRepository.GetReportsForCleanup(cutoffUtc, cancellationToken);
        if (oldReports.Count == 0)
        {
            return;
        }

        var deletedIds = new List<Guid>();
        foreach (var report in oldReports)
        {
            try
            {
                await _minioService.RemoveFileAsync(bucket, report.ObjectName, cancellationToken);
                deletedIds.Add(report.Id);
            }
            catch
            {
                // Ошибки удаления не останавливают очистку.
            }
        }

        if (deletedIds.Count > 0)
        {
            await _weeklyAdminReportRepository.MarkReportsDeleted(
                deletedIds,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);
        }
    }
}
