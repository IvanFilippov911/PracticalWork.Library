using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Models.ReportModels;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий данных для еженедельного административного отчета.
/// </summary>
public sealed class WeeklyAdminReportRepository : IWeeklyAdminReportRepository
{
    private readonly AppDbContext _appDbContext;

    public WeeklyAdminReportRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<WeeklyAdminReportStatistics> GetStatistics(
        DateOnly periodFrom,
        DateOnly periodTo,
        TimeZoneInfo timeZone,
        CancellationToken cancellationToken = default)
    {
        var periodFromUtc = TimeZoneInfo.ConvertTimeToUtc(periodFrom.ToDateTime(TimeOnly.MinValue), timeZone);
        var periodToExclusiveUtc = TimeZoneInfo.ConvertTimeToUtc(periodTo.AddDays(1).ToDateTime(TimeOnly.MinValue), timeZone);
        var newBooksCount = await _appDbContext.Books
            .AsNoTracking()
            .CountAsync(
                book => book.CreatedAt >= periodFromUtc && book.CreatedAt < periodToExclusiveUtc,
                cancellationToken);

        var newReadersCount = await _appDbContext.Readers
            .AsNoTracking()
            .CountAsync(
                reader => reader.CreatedAt >= periodFromUtc && reader.CreatedAt < periodToExclusiveUtc,
                cancellationToken);

        var borrowStatistics = await _appDbContext.BookBorrows
            .AsNoTracking()
            .Where(borrow =>
                (borrow.BorrowDate >= periodFrom && borrow.BorrowDate <= periodTo)
                || (borrow.ReturnDate >= periodFrom && borrow.ReturnDate <= periodTo && borrow.Status != BookIssueStatus.Issued)
                || (borrow.Status == BookIssueStatus.Issued && borrow.DueDate < periodTo))
            .GroupBy(_ => 1)
            .Select(group => new
            {
                BorrowedCount = group.Count(borrow => borrow.BorrowDate >= periodFrom && borrow.BorrowDate <= periodTo),
                ReturnedCount = group.Count(
                    borrow => borrow.ReturnDate >= periodFrom
                              && borrow.ReturnDate <= periodTo
                              && borrow.Status != BookIssueStatus.Issued),
                OverdueCount = group.Count(
                    borrow => borrow.Status == BookIssueStatus.Issued
                              && borrow.DueDate < periodTo)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return new WeeklyAdminReportStatistics
        {
            NewBooksCount = newBooksCount,
            NewReadersCount = newReadersCount,
            BorrowedCount = borrowStatistics?.BorrowedCount ?? 0,
            ReturnedCount = borrowStatistics?.ReturnedCount ?? 0,
            OverdueCount = borrowStatistics?.OverdueCount ?? 0
        };
    }

    public async Task UpsertMetadata(
        WeeklyAdminReportMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var existing = await _appDbContext.WeeklyReportMetadata
            .SingleOrDefaultAsync(report => report.ReportName == metadata.ReportName, cancellationToken);

        if (existing is null)
        {
            await _appDbContext.WeeklyReportMetadata.AddAsync(new WeeklyReportMetadataEntity
            {
                ReportName = metadata.ReportName,
                BucketName = metadata.BucketName,
                ObjectName = metadata.ObjectName,
                PeriodFrom = metadata.PeriodFrom,
                PeriodTo = metadata.PeriodTo,
                NewBooksCount = metadata.Statistics.NewBooksCount,
                NewReadersCount = metadata.Statistics.NewReadersCount,
                BorrowedCount = metadata.Statistics.BorrowedCount,
                ReturnedCount = metadata.Statistics.ReturnedCount,
                OverdueCount = metadata.Statistics.OverdueCount,
                IsDeleted = false
            }, cancellationToken);
        }
        else
        {
            existing.BucketName = metadata.BucketName;
            existing.ObjectName = metadata.ObjectName;
            existing.PeriodFrom = metadata.PeriodFrom;
            existing.PeriodTo = metadata.PeriodTo;
            existing.NewBooksCount = metadata.Statistics.NewBooksCount;
            existing.NewReadersCount = metadata.Statistics.NewReadersCount;
            existing.BorrowedCount = metadata.Statistics.BorrowedCount;
            existing.ReturnedCount = metadata.Statistics.ReturnedCount;
            existing.OverdueCount = metadata.Statistics.OverdueCount;
            existing.IsDeleted = false;
            existing.DeletedAt = null;
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WeeklyAdminReportCleanupCandidate>> GetReportsForCleanup(
        DateTime cutoffUtc,
        CancellationToken cancellationToken = default)
    {
        return await _appDbContext.WeeklyReportMetadata
            .AsNoTracking()
            .Where(report => !report.IsDeleted && report.CreatedAt < cutoffUtc)
            .Select(report => new WeeklyAdminReportCleanupCandidate
            {
                Id = report.Id,
                ReportName = report.ReportName,
                ObjectName = report.ObjectName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task MarkReportsDeleted(
        IReadOnlyCollection<Guid> reportIds,
        DateTime deletedAt,
        CancellationToken cancellationToken = default)
    {
        var reports = await _appDbContext.WeeklyReportMetadata
            .Where(report => reportIds.Contains(report.Id))
            .ToListAsync(cancellationToken);

        foreach (var report in reports)
        {
            report.IsDeleted = true;
            report.DeletedAt = deletedAt;
        }

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveNotificationLogs(
        IReadOnlyCollection<WeeklyAdminReportNotificationLogEntry> logs,
        CancellationToken cancellationToken = default)
    {
        var entities = logs
            .Select(log => new NotificationLogEntity
            {
                NotificationType = NotificationTypes.WeeklyReport,
                ReceiverEmail = log.ReceiverEmail,
                SentAt = log.SentAt,
                IsSuccess = log.IsSuccess,
                ErrorMessage = log.ErrorMessage
            })
            .ToList();

        await _appDbContext.NotificationLogs.AddRangeAsync(entities, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}
