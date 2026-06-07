using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис сценария отправки напоминаний о возврате книг.
/// </summary>
public sealed class ReturnReminderProcessingService : IReturnReminderProcessingService
{
    private readonly IReturnReminderRepository _returnReminderRepository;
    private readonly IReturnReminderEmailBuilder _returnReminderEmailBuilder;
    private readonly IEmailService _emailService;
    private readonly TimeProvider _timeProvider;

    public ReturnReminderProcessingService(
        IReturnReminderRepository returnReminderRepository,
        IReturnReminderEmailBuilder returnReminderEmailBuilder,
        IEmailService emailService,
        TimeProvider timeProvider)
    {
        _returnReminderRepository = returnReminderRepository;
        _returnReminderEmailBuilder = returnReminderEmailBuilder;
        _emailService = emailService;
        _timeProvider = timeProvider;
    }

    public async Task<ReturnReminderProcessingResult> ProcessAsync(
        DateOnly dueDate,
        DateTime duplicateCutoff,
        ReturnReminderTemplate template,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _returnReminderRepository.GetCandidates(
            dueDate,
            duplicateCutoff,
            cancellationToken);
        if (candidates.Count == 0)
        {
            return new ReturnReminderProcessingResult();
        }

        var successCount = 0;
        var failureCount = 0;
        var skippedCount = 0;
        var notificationLogs = new List<ReturnReminderNotificationLogEntry>();

        foreach (var candidate in candidates)
        {
            if (candidate.HasRecentSuccessfulReminder)
            {
                skippedCount++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(candidate.ReaderEmail))
            {
                failureCount++;
                notificationLogs.Add(CreateNotificationLog(
                    candidate,
                    false,
                    "У читателя не указан email"));
                continue;
            }

            var message = await _returnReminderEmailBuilder.BuildAsync(candidate, template, cancellationToken);
            var sendResult = await _emailService.SendAsync(message, cancellationToken);

            if (sendResult.Success)
            {
                successCount++;
            }
            else
            {
                failureCount++;
            }

            notificationLogs.Add(CreateNotificationLog(candidate, sendResult.Success, sendResult.ErrorMessage));
        }

        if (notificationLogs.Count > 0)
        {
            await _returnReminderRepository.SaveNotificationLogs(notificationLogs, cancellationToken);
        }

        return new ReturnReminderProcessingResult
        {
            CandidateCount = candidates.Count,
            SuccessCount = successCount,
            FailureCount = failureCount,
            SkippedCount = skippedCount
        };
    }

    private ReturnReminderNotificationLogEntry CreateNotificationLog(
        ReturnReminderCandidate candidate,
        bool isSuccess,
        string errorMessage)
    {
        return new ReturnReminderNotificationLogEntry
        {
            ReaderId = candidate.ReaderId,
            BookId = candidate.BookId,
            BorrowId = candidate.BorrowId,
            ReceiverEmail = candidate.ReaderEmail,
            SentAt = _timeProvider.GetUtcNow().UtcDateTime,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage
        };
    }
}
