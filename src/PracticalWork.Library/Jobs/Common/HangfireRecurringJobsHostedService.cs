using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Options;
using PracticalWork.Library.Jobs.Archive;
using PracticalWork.Library.Jobs.Notifications;
using PracticalWork.Library.Jobs.Reports;

namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Регистрирует recurring-задачи Hangfire при запуске приложения.
/// </summary>
public sealed class HangfireRecurringJobsHostedService : IHostedService
{
    private readonly JobSettings _jobSettings;
    private readonly ILogger<HangfireRecurringJobsHostedService> _logger;

    public HangfireRecurringJobsHostedService(
        IOptions<JobSettings> jobSettingsOptions,
        ILogger<HangfireRecurringJobsHostedService> logger)
    {
        _jobSettings = jobSettingsOptions.Value;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ValidateSettings(_jobSettings);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_jobSettings.TimeZoneId);

        RecurringJob.AddOrUpdate<ReturnReminderJob>(
            LibraryJobNames.ReturnReminder,
            job => job.ExecuteAsync(CancellationToken.None),
            _jobSettings.Jobs[LibraryJobNames.ReturnReminder].CronExpression,
            new RecurringJobOptions
            {
                TimeZone = timeZone
            });

        RecurringJob.AddOrUpdate<WeeklyAdminReportJob>(
            LibraryJobNames.WeeklyAdminReport,
            job => job.ExecuteAsync(CancellationToken.None),
            _jobSettings.Jobs[LibraryJobNames.WeeklyAdminReport].CronExpression,
            new RecurringJobOptions
            {
                TimeZone = timeZone
            });

        RecurringJob.AddOrUpdate<ArchiveOldBooksJob>(
            LibraryJobNames.ArchiveOldBooks,
            job => job.ExecuteAsync(CancellationToken.None),
            _jobSettings.Jobs[LibraryJobNames.ArchiveOldBooks].CronExpression,
            new RecurringJobOptions
            {
                TimeZone = timeZone
            });

        _logger.LogInformation("Hangfire recurring jobs configured for timezone {TimeZone}", _jobSettings.TimeZoneId);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static void ValidateSettings(JobSettings settings)
    {
        foreach (var jobName in LibraryJobNames.All)
        {
            if (!settings.Jobs.TryGetValue(jobName, out var configuration))
            {
                throw new InvalidOperationException($"Отсутствует конфигурация задачи '{jobName}' в App:Jobs:Jobs");
            }

            if (!CronExpressionValidator.TryValidate(configuration.CronExpression, out var error))
            {
                throw new InvalidOperationException(
                    $"Некорректный cron для '{jobName}': {configuration.CronExpression}. {error}");
            }

            if (configuration.TimeoutMinutes <= 0)
            {
                throw new InvalidOperationException($"TimeoutMinutes для '{jobName}' должен быть > 0");
            }

            if (configuration.MaxRetries < 0)
            {
                throw new InvalidOperationException($"MaxRetries для '{jobName}' не может быть < 0");
            }
        }
    }
}
