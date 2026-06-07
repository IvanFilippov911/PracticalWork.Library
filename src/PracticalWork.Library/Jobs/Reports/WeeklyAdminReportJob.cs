using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Jobs.Common;
using PracticalWork.Library.Models.ReportModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Jobs.Reports;

/// <summary>
/// Еженедельный отчет для администрации библиотеки.
/// </summary>
public sealed class WeeklyAdminReportJob : ILibraryJob
{
    private readonly IWeeklyAdminReportProcessingService _weeklyAdminReportProcessingService;
    private readonly JobSettings _jobSettings;
    private readonly EmailSettings _emailSettings;
    private readonly EmailTemplateSettings _templateSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<WeeklyAdminReportJob> _logger;

    public WeeklyAdminReportJob(
        IWeeklyAdminReportProcessingService weeklyAdminReportProcessingService,
        IOptions<JobSettings> jobSettingsOptions,
        IOptions<EmailSettings> emailSettingsOptions,
        IOptions<EmailTemplateSettings> templateSettingsOptions,
        TimeProvider timeProvider,
        ILogger<WeeklyAdminReportJob> logger)
    {
        _weeklyAdminReportProcessingService = weeklyAdminReportProcessingService;
        _jobSettings = jobSettingsOptions.Value;
        _emailSettings = emailSettingsOptions.Value;
        _templateSettings = templateSettingsOptions.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public string JobName => LibraryJobNames.WeeklyAdminReport;

    public string Description => LibraryJobDescriptions.GetDescription(LibraryJobNames.WeeklyAdminReport);

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var configuration = _jobSettings.Jobs[JobName];
        await JobExecutionPolicy.ExecuteAsync(JobName, configuration, _logger, ExecuteCoreAsync, cancellationToken);
    }

    private async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var result = await _weeklyAdminReportProcessingService.ProcessAsync(
            new WeeklyAdminReportProcessingRequest
            {
                TimeZoneId = _jobSettings.TimeZoneId,
                Template = _templateSettings.WeeklyReport,
                EmailSettings = _emailSettings
            },
            cancellationToken);

        if (result.AdminEmailCount == 0)
        {
            _logger.LogInformation("{JobName}: список администраторов пуст, отправка email пропущена", JobName);
            return;
        }

        _logger.LogInformation(
            "{JobName}: отчет {ReportName} создан и отправлен {Count} администраторам. Success={Success}, Failed={Failed}",
            JobName,
            result.ReportName,
            result.AdminEmailCount,
            result.SuccessCount,
            result.FailureCount);
    }
}
