using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Jobs.Common;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Jobs.Notifications;

/// <summary>
/// Ежедневная задача отправки напоминаний о возврате книг.
/// </summary>
public sealed class ReturnReminderJob : ILibraryJob
{
    private readonly IReturnReminderProcessingService _returnReminderProcessingService;
    private readonly JobSettings _jobSettings;
    private readonly EmailTemplateSettings _templateSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ReturnReminderJob> _logger;

    public ReturnReminderJob(
        IReturnReminderProcessingService returnReminderProcessingService,
        IOptions<JobSettings> jobSettingsOptions,
        IOptions<EmailTemplateSettings> templateSettingsOptions,
        TimeProvider timeProvider,
        ILogger<ReturnReminderJob> logger)
    {
        _returnReminderProcessingService = returnReminderProcessingService;
        _jobSettings = jobSettingsOptions.Value;
        _templateSettings = templateSettingsOptions.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public string JobName => LibraryJobNames.ReturnReminder;

    public string Description => LibraryJobDescriptions.GetDescription(LibraryJobNames.ReturnReminder);

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var configuration = _jobSettings.Jobs[JobName];
        await JobExecutionPolicy.ExecuteAsync(JobName, configuration, _logger, ExecuteCoreAsync, cancellationToken);
    }

    private async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var template = _templateSettings.ReturnReminder;
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var dueDate = DateOnly.FromDateTime(utcNow.AddDays(template.DaysBeforeDueDate));
        var duplicateCutoff = utcNow.AddHours(-24);

        var result = await _returnReminderProcessingService.ProcessAsync(
            dueDate,
            duplicateCutoff,
            template,
            cancellationToken);

        if (result.CandidateCount == 0)
        {
            _logger.LogInformation("{JobName}: кандидаты на отправку не найдены", JobName);
            return;
        }

        _logger.LogInformation(
            "{JobName}: завершено. Success={Success}, Failed={Failed}, Skipped={Skipped}",
            JobName,
            result.SuccessCount,
            result.FailureCount,
            result.SkippedCount);
    }
}
