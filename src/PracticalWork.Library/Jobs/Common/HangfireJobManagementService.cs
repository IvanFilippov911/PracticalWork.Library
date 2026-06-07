using Hangfire;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.JobModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Сервис управления задачами Hangfire.
/// </summary>
public sealed class HangfireJobManagementService : IJobManagementService
{
    private readonly JobSettings _jobSettings;

    public HangfireJobManagementService(IOptions<JobSettings> jobSettingsOptions)
    {
        _jobSettings = jobSettingsOptions.Value;
    }

    public IReadOnlyList<ScheduledJobInfo> GetJobs()
    {
        return LibraryJobNames.All
            .Where(jobName => _jobSettings.Jobs.ContainsKey(jobName))
            .Select(jobName => new ScheduledJobInfo
            {
                Name = jobName,
                Description = LibraryJobDescriptions.GetDescription(jobName),
                CronExpression = _jobSettings.Jobs[jobName].CronExpression,
                TimeZoneId = _jobSettings.TimeZoneId
            })
            .ToList();
    }

    public Task TriggerAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var actualJobName = LibraryJobNames.All
            .FirstOrDefault(name => string.Equals(name, jobName, StringComparison.OrdinalIgnoreCase));

        if (actualJobName is null)
        {
            throw new ArgumentException($"Неизвестная задача: {jobName}", nameof(jobName));
        }

        RecurringJob.TriggerJob(actualJobName);
        return Task.CompletedTask;
    }
}
