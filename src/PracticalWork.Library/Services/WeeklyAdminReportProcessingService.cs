using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис сценария подготовки и рассылки еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportProcessingService : IWeeklyAdminReportProcessingService
{
    private readonly IWeeklyAdminReportRepository _weeklyAdminReportRepository;
    private readonly IWeeklyAdminReportPeriodService _weeklyAdminReportPeriodService;
    private readonly IWeeklyAdminReportStorageService _weeklyAdminReportStorageService;
    private readonly IWeeklyAdminReportNotificationService _weeklyAdminReportNotificationService;

    public WeeklyAdminReportProcessingService(
        IWeeklyAdminReportRepository weeklyAdminReportRepository,
        IWeeklyAdminReportPeriodService weeklyAdminReportPeriodService,
        IWeeklyAdminReportStorageService weeklyAdminReportStorageService,
        IWeeklyAdminReportNotificationService weeklyAdminReportNotificationService)
    {
        _weeklyAdminReportRepository = weeklyAdminReportRepository;
        _weeklyAdminReportPeriodService = weeklyAdminReportPeriodService;
        _weeklyAdminReportStorageService = weeklyAdminReportStorageService;
        _weeklyAdminReportNotificationService = weeklyAdminReportNotificationService;
    }

    public async Task<WeeklyAdminReportProcessingResult> ProcessAsync(
        WeeklyAdminReportProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        var period = _weeklyAdminReportPeriodService.GetPreviousWeek(request.TimeZoneId);
        var statistics = await _weeklyAdminReportRepository.GetStatistics(
            period.PeriodFrom,
            period.PeriodTo,
            period.TimeZone,
            cancellationToken);

        var artifact = await _weeklyAdminReportStorageService.PublishAsync(
            request,
            period,
            statistics,
            cancellationToken);

        await _weeklyAdminReportStorageService.CleanupAsync(
            artifact.BucketName,
            request.Template.ReportRetentionDays,
            cancellationToken);

        var notificationSummary = await _weeklyAdminReportNotificationService.SendAsync(
            request,
            period,
            statistics,
            artifact.ReportUrl,
            cancellationToken);

        return new WeeklyAdminReportProcessingResult
        {
            ReportName = artifact.ReportName,
            AdminEmailCount = notificationSummary.AdminEmailCount,
            SuccessCount = notificationSummary.SuccessCount,
            FailureCount = notificationSummary.FailureCount
        };
    }
}
