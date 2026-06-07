using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий данных для архивации книг.
/// </summary>
public sealed class BookArchiveRepository : IBookArchiveRepository
{
    private readonly AppDbContext _appDbContext;

    public BookArchiveRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArchiveBookCandidate>> GetArchiveCandidates(
        DateOnly thresholdDate,
        int maxBooks,
        CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Books
            .AsNoTracking()
            .Where(book => book.Status != BookStatus.Archived)
            .Select(book => new ArchiveBookCandidate
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BookStatus = book.Status,
                LastBorrowDate = _appDbContext.BookBorrows
                    .Where(borrow => borrow.BookId == book.Id)
                    .Max(borrow => (DateOnly?)borrow.BorrowDate),
                HasActiveBorrow = _appDbContext.BookBorrows
                    .Any(borrow => borrow.BookId == book.Id && borrow.Status == BookIssueStatus.Issued)
            })
            .Where(candidate => candidate.LastBorrowDate == null || candidate.LastBorrowDate < thresholdDate)
            .OrderBy(candidate => candidate.LastBorrowDate ?? DateOnly.MinValue)
            .Take(maxBooks)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlySet<Guid>> GetBooksWithActiveBorrow(
        IReadOnlyCollection<Guid> bookIds,
        CancellationToken cancellationToken = default)
    {
        var result = await _appDbContext.BookBorrows
            .AsNoTracking()
            .Where(borrow => bookIds.Contains(borrow.BookId) && borrow.Status == BookIssueStatus.Issued)
            .Select(borrow => borrow.BookId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return result.ToHashSet();
    }

    /// <inheritdoc />
    public async Task SaveArchiveProcessingResult(
        ArchiveJobRun jobRun,
        IReadOnlyCollection<ArchiveBookLogEntry> logs,
        CancellationToken cancellationToken = default)
    {
        var jobRunEntity = new ArchiveJobRunEntity
        {
            Id = jobRun.Id,
            ThresholdDate = jobRun.ThresholdDate,
            MaxBooksPerRun = jobRun.MaxBooksPerRun,
            ProcessedCount = jobRun.ProcessedCount,
            ArchivedCount = jobRun.ArchivedCount,
            SkippedCount = jobRun.SkippedCount,
            FailedCount = jobRun.FailedCount,
            StartedAt = jobRun.StartedAt,
            CompletedAt = jobRun.CompletedAt,
            DurationMs = jobRun.DurationMs
        };

        var entities = logs
            .Select(log => new ArchiveLogEntity
            {
                JobRunId = log.JobRunId,
                BookId = log.BookId,
                BookTitle = log.BookTitle,
                Status = log.Status,
                Reason = log.Reason,
                ProcessedAt = log.ProcessedAt
            })
            .ToList();

        await _appDbContext.ArchiveJobRuns.AddAsync(jobRunEntity, cancellationToken);
        await _appDbContext.ArchiveLogs.AddRangeAsync(entities, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}
