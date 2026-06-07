using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис сценария архивации старых книг.
/// </summary>
public sealed class BookArchiveProcessingService : IBookArchiveProcessingService
{
    private readonly IBookArchiveRepository _bookArchiveRepository;
    private readonly IBookService _bookService;
    private readonly TimeProvider _timeProvider;

    public BookArchiveProcessingService(
        IBookArchiveRepository bookArchiveRepository,
        IBookService bookService,
        TimeProvider timeProvider)
    {
        _bookArchiveRepository = bookArchiveRepository;
        _bookService = bookService;
        _timeProvider = timeProvider;
    }

    public async Task<ArchiveBooksProcessingResult> ProcessAsync(
        DateOnly thresholdDate,
        int maxBooks,
        CancellationToken cancellationToken = default)
    {
        var runId = Guid.NewGuid();
        var startTime = _timeProvider.GetUtcNow().UtcDateTime;
        var candidates = await _bookArchiveRepository.GetArchiveCandidates(
            thresholdDate,
            maxBooks,
            cancellationToken);

        if (candidates.Count == 0)
        {
            var emptyCompletedAt = _timeProvider.GetUtcNow().UtcDateTime;
            var emptyResult = new ArchiveBooksProcessingResult
            {
                ProcessedCount = 0,
                Duration = emptyCompletedAt - startTime
            };

            await _bookArchiveRepository.SaveArchiveProcessingResult(
                CreateJobRun(runId, thresholdDate, maxBooks, emptyResult, startTime, emptyCompletedAt),
                Array.Empty<ArchiveBookLogEntry>(),
                cancellationToken);

            return emptyResult;
        }

        var archivedCount = 0;
        var skippedCount = 0;
        var failedCount = 0;
        var logs = new List<ArchiveBookLogEntry>();
        var bookIds = candidates.Select(candidate => candidate.BookId).ToList();
        var booksWithActiveBorrow = await _bookArchiveRepository.GetBooksWithActiveBorrow(
            bookIds,
            cancellationToken);

        foreach (var candidate in candidates)
        {
            var result = await ProcessCandidateAsync(
                runId,
                candidate,
                booksWithActiveBorrow.Contains(candidate.BookId),
                cancellationToken);

            archivedCount += result.ArchivedCount;
            skippedCount += result.SkippedCount;
            failedCount += result.FailedCount;
            logs.Add(result.LogEntry);
        }

        var completedAt = _timeProvider.GetUtcNow().UtcDateTime;
        var duration = completedAt - startTime;

        var processingResult = new ArchiveBooksProcessingResult
        {
            ProcessedCount = candidates.Count,
            ArchivedCount = archivedCount,
            SkippedCount = skippedCount,
            FailedCount = failedCount,
            Duration = duration
        };

        await _bookArchiveRepository.SaveArchiveProcessingResult(
            CreateJobRun(runId, thresholdDate, maxBooks, processingResult, startTime, completedAt),
            logs,
            cancellationToken);

        return processingResult;
    }

    private async Task<ArchiveCandidateProcessingResult> ProcessCandidateAsync(
        Guid runId,
        ArchiveBookCandidate candidate,
        bool hasActiveBorrowAtProcessingStart,
        CancellationToken cancellationToken)
    {
        if (candidate.BookStatus != BookStatus.Available)
        {
            return CreateCandidateResult(
                runId,
                candidate,
                "skipped",
                $"Текущий статус книги '{candidate.BookStatus}', ожидался '{BookStatus.Available}'",
                skippedCount: 1);
        }

        if (candidate.HasActiveBorrow)
        {
            return CreateCandidateResult(
                runId,
                candidate,
                "skipped",
                "Книга выдана читателю",
                skippedCount: 1);
        }

        if (hasActiveBorrowAtProcessingStart)
        {
            return CreateCandidateResult(
                runId,
                candidate,
                "skipped",
                "Книга выдана читателю на момент обработки",
                skippedCount: 1);
        }

        try
        {
            await _bookService.ArchiveBook(candidate.BookId);
            return CreateCandidateResult(
                runId,
                candidate,
                "archived",
                null,
                archivedCount: 1);
        }
        catch (Exception exception)
        {
            return CreateCandidateResult(
                runId,
                candidate,
                "failed",
                exception.Message,
                failedCount: 1);
        }
    }

    private ArchiveCandidateProcessingResult CreateCandidateResult(
        Guid runId,
        ArchiveBookCandidate candidate,
        string status,
        string reason,
        int archivedCount = 0,
        int skippedCount = 0,
        int failedCount = 0)
    {
        return new ArchiveCandidateProcessingResult
        {
            ArchivedCount = archivedCount,
            SkippedCount = skippedCount,
            FailedCount = failedCount,
            LogEntry = CreateBookLog(runId, candidate, status, reason)
        };
    }

    private ArchiveBookLogEntry CreateBookLog(
        Guid runId,
        ArchiveBookCandidate candidate,
        string status,
        string reason)
    {
        return new ArchiveBookLogEntry
        {
            JobRunId = runId,
            BookId = candidate.BookId,
            BookTitle = candidate.BookTitle,
            Status = status,
            Reason = reason,
            ProcessedAt = _timeProvider.GetUtcNow().UtcDateTime
        };
    }

    private static ArchiveJobRun CreateJobRun(
        Guid runId,
        DateOnly thresholdDate,
        int maxBooks,
        ArchiveBooksProcessingResult result,
        DateTime startedAt,
        DateTime completedAt)
    {
        return new ArchiveJobRun
        {
            Id = runId,
            ThresholdDate = thresholdDate,
            MaxBooksPerRun = maxBooks,
            ProcessedCount = result.ProcessedCount,
            ArchivedCount = result.ArchivedCount,
            SkippedCount = result.SkippedCount,
            FailedCount = result.FailedCount,
            StartedAt = startedAt,
            CompletedAt = completedAt,
            DurationMs = (long)result.Duration.TotalMilliseconds
        };
    }

    private sealed class ArchiveCandidateProcessingResult
    {
        public int ArchivedCount { get; init; }
        public int SkippedCount { get; init; }
        public int FailedCount { get; init; }
        public required ArchiveBookLogEntry LogEntry { get; init; }
    }
}
