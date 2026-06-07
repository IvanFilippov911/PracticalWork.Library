using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class ReturnReminderProcessingServiceTests
{
    [Fact]
    public async Task ProcessAsync_WhenNoCandidates_ReturnsEmptyResultWithoutExternalCalls()
    {
        var repository = new Mock<IReturnReminderRepository>();
        repository.Setup(x => x.GetCandidates(It.IsAny<DateOnly>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var builder = new Mock<IReturnReminderEmailBuilder>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var service = new ReturnReminderProcessingService(
            repository.Object,
            builder.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(
            new DateOnly(2026, 5, 29),
            new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            new ReturnReminderTemplate());

        Assert.Equal(0, result.CandidateCount);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.Equal(0, result.SkippedCount);
        repository.Verify(x => x.SaveNotificationLogs(It.IsAny<IReadOnlyCollection<ReturnReminderNotificationLogEntry>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_WhenCandidateAlreadyReminded_SkipsSending()
    {
        var repository = new Mock<IReturnReminderRepository>();
        repository.Setup(x => x.GetCandidates(It.IsAny<DateOnly>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ReturnReminderCandidate
                {
                    BorrowId = Guid.NewGuid(),
                    ReaderId = Guid.NewGuid(),
                    ReaderFullName = "Reader",
                    ReaderEmail = "reader@test.local",
                    BookId = Guid.NewGuid(),
                    BookTitle = "Book",
                    BookAuthors = ["Author"],
                    DueDate = new DateOnly(2026, 5, 29),
                    HasRecentSuccessfulReminder = true
                }
            ]);
        var builder = new Mock<IReturnReminderEmailBuilder>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var service = new ReturnReminderProcessingService(
            repository.Object,
            builder.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(
            new DateOnly(2026, 5, 29),
            DateTime.UtcNow,
            new ReturnReminderTemplate());

        Assert.Equal(1, result.CandidateCount);
        Assert.Equal(1, result.SkippedCount);
        repository.Verify(x => x.SaveNotificationLogs(It.IsAny<IReadOnlyCollection<ReturnReminderNotificationLogEntry>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_WhenCandidateHasNoEmail_RecordsFailureLog()
    {
        var repository = new Mock<IReturnReminderRepository>();
        IReadOnlyCollection<ReturnReminderNotificationLogEntry>? savedLogs = null;
        repository.Setup(x => x.GetCandidates(It.IsAny<DateOnly>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ReturnReminderCandidate
                {
                    BorrowId = Guid.NewGuid(),
                    ReaderId = Guid.NewGuid(),
                    ReaderFullName = "Reader",
                    ReaderEmail = "",
                    BookId = Guid.NewGuid(),
                    BookTitle = "Book",
                    BookAuthors = ["Author"],
                    DueDate = new DateOnly(2026, 5, 29)
                }
            ]);
        repository.Setup(x => x.SaveNotificationLogs(It.IsAny<IReadOnlyCollection<ReturnReminderNotificationLogEntry>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<ReturnReminderNotificationLogEntry>, CancellationToken>((logs, _) => savedLogs = logs);

        var service = new ReturnReminderProcessingService(
            repository.Object,
            new Mock<IReturnReminderEmailBuilder>(MockBehavior.Strict).Object,
            new Mock<IEmailService>(MockBehavior.Strict).Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(
            new DateOnly(2026, 5, 29),
            DateTime.UtcNow,
            new ReturnReminderTemplate());

        Assert.Equal(1, result.CandidateCount);
        Assert.Equal(1, result.FailureCount);
        Assert.NotNull(savedLogs);
        Assert.Single(savedLogs!);
        Assert.Equal("У читателя не указан email", savedLogs!.Single().ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_WhenCandidatesMixed_AggregatesCountsAndSavesLogs()
    {
        var successCandidate = new ReturnReminderCandidate
        {
            BorrowId = Guid.NewGuid(),
            ReaderId = Guid.NewGuid(),
            ReaderFullName = "Success Reader",
            ReaderEmail = "success@test.local",
            BookId = Guid.NewGuid(),
            BookTitle = "Book 1",
            BookAuthors = ["Author"],
            DueDate = new DateOnly(2026, 5, 29)
        };
        var failedCandidate = new ReturnReminderCandidate
        {
            BorrowId = Guid.NewGuid(),
            ReaderId = Guid.NewGuid(),
            ReaderFullName = "Fail Reader",
            ReaderEmail = "fail@test.local",
            BookId = Guid.NewGuid(),
            BookTitle = "Book 2",
            BookAuthors = ["Author"],
            DueDate = new DateOnly(2026, 5, 29)
        };
        var repository = new Mock<IReturnReminderRepository>();
        repository.Setup(x => x.GetCandidates(It.IsAny<DateOnly>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([successCandidate, failedCandidate]);
        IReadOnlyCollection<ReturnReminderNotificationLogEntry>? savedLogs = null;
        repository.Setup(x => x.SaveNotificationLogs(It.IsAny<IReadOnlyCollection<ReturnReminderNotificationLogEntry>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyCollection<ReturnReminderNotificationLogEntry>, CancellationToken>((logs, _) => savedLogs = logs);

        var builder = new Mock<IReturnReminderEmailBuilder>();
        builder.Setup(x => x.BuildAsync(It.IsAny<ReturnReminderCandidate>(), It.IsAny<ReturnReminderTemplate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReturnReminderCandidate candidate, ReturnReminderTemplate _, CancellationToken _) => new EmailMessage
            {
                To = candidate.ReaderEmail,
                Subject = "Reminder",
                TextBody = "Body"
            });

        var emailService = new Mock<IEmailService>();
        emailService.Setup(x => x.SendAsync(It.Is<EmailMessage>(m => m.To == "success@test.local"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(true));
        emailService.Setup(x => x.SendAsync(It.Is<EmailMessage>(m => m.To == "fail@test.local"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailSendResult(false, "smtp error"));

        var service = new ReturnReminderProcessingService(
            repository.Object,
            builder.Object,
            emailService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(
            new DateOnly(2026, 5, 29),
            DateTime.UtcNow,
            new ReturnReminderTemplate());

        Assert.Equal(2, result.CandidateCount);
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        Assert.NotNull(savedLogs);
        Assert.Equal(2, savedLogs!.Count);
    }
}
