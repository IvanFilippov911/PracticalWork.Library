using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Jobs.Common;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Jobs.Archive;

/// <summary>
/// Ежемесячная архивация старых книг.
/// </summary>
public sealed class ArchiveOldBooksJob : ILibraryJob
{
    private readonly IBookArchiveProcessingService _bookArchiveProcessingService;
    private readonly JobSettings _jobSettings;
    private readonly ArchiveSettings _archiveSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ArchiveOldBooksJob> _logger;

    public ArchiveOldBooksJob(
        IBookArchiveProcessingService bookArchiveProcessingService,
        IOptions<JobSettings> jobSettingsOptions,
        IOptions<ArchiveSettings> archiveSettingsOptions,
        TimeProvider timeProvider,
        ILogger<ArchiveOldBooksJob> logger)
    {
        _bookArchiveProcessingService = bookArchiveProcessingService;
        _jobSettings = jobSettingsOptions.Value;
        _archiveSettings = archiveSettingsOptions.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public string JobName => LibraryJobNames.ArchiveOldBooks;

    public string Description => LibraryJobDescriptions.GetDescription(LibraryJobNames.ArchiveOldBooks);

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var configuration = _jobSettings.Jobs[JobName];
        await JobExecutionPolicy.ExecuteAsync(JobName, configuration, _logger, ExecuteCoreAsync, cancellationToken);
    }

    private async Task ExecuteCoreAsync(CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var thresholdDate = DateOnly.FromDateTime(utcNow.Date.AddYears(-_archiveSettings.YearsWithoutBorrow));
        var maxBooks = Math.Max(1, _archiveSettings.MaxBooksPerRun);

        var result = await _bookArchiveProcessingService.ProcessAsync(
            thresholdDate,
            maxBooks,
            cancellationToken);

        if (result.ProcessedCount == 0)
        {
            _logger.LogInformation("{JobName}: книги для архивации не найдены", JobName);
            return;
        }

        _logger.LogInformation(
            "{JobName}: processed={Processed}; archived={Archived}; skipped={Skipped}; failed={Failed}; duration={Duration}",
            JobName,
            result.ProcessedCount,
            result.ArchivedCount,
            result.SkippedCount,
            result.FailedCount,
            result.Duration);
    }
}
