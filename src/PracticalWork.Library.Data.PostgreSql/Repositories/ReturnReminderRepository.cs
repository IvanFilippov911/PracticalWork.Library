using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.NotificationModels;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий данных для напоминаний о возврате книг.
/// </summary>
public sealed class ReturnReminderRepository : IReturnReminderRepository
{
    private readonly AppDbContext _appDbContext;

    public ReturnReminderRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IReadOnlyList<ReturnReminderCandidate>> GetCandidates(
        DateOnly dueDate,
        DateTime duplicateCutoff,
        CancellationToken cancellationToken = default)
    {
        return await (
            from borrow in _appDbContext.Set<BookBorrowEntity>().AsNoTracking()
            join reader in _appDbContext.Set<ReaderEntity>().AsNoTracking() on borrow.ReaderId equals reader.Id
            join book in _appDbContext.Set<AbstractBookEntity>().AsNoTracking() on borrow.BookId equals book.Id
            where borrow.Status == BookIssueStatus.Issued
                  && borrow.DueDate == dueDate
                  && reader.IsActive
            select new ReturnReminderCandidate
            {
                BorrowId = borrow.Id,
                ReaderId = reader.Id,
                ReaderFullName = reader.FullName,
                ReaderEmail = reader.Email,
                BookId = book.Id,
                BookTitle = book.Title,
                BookAuthors = book.Authors.ToArray(),
                DueDate = borrow.DueDate,
                HasRecentSuccessfulReminder = _appDbContext.NotificationLogs
                    .Any(log => log.NotificationType == NotificationTypes.ReturnReminder
                                && log.IsSuccess
                                && log.BorrowId == borrow.Id
                                && log.SentAt >= duplicateCutoff)
            }).ToListAsync(cancellationToken);
    }

    public async Task SaveNotificationLogs(
        IReadOnlyCollection<ReturnReminderNotificationLogEntry> logs,
        CancellationToken cancellationToken = default)
    {
        var entities = logs
            .Select(log => new NotificationLogEntity
            {
                NotificationType = NotificationTypes.ReturnReminder,
                ReaderId = log.ReaderId,
                BookId = log.BookId,
                BorrowId = log.BorrowId,
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
