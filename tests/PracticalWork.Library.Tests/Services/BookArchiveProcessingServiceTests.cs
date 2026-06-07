using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Services;
using PracticalWork.Tests.Common;

namespace PracticalWork.Library.Tests.Services;

public sealed class BookArchiveProcessingServiceTests
{
    [Fact]
    public async Task ProcessAsync_WhenNoCandidates_SavesEmptyRun()
    {
        var repository = new Mock<IBookArchiveRepository>();
        repository.Setup(x => x.GetArchiveCandidates(It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        ArchiveJobRun? savedRun = null;
        IReadOnlyCollection<ArchiveBookLogEntry>? savedLogs = null;
        repository.Setup(x => x.SaveArchiveProcessingResult(It.IsAny<ArchiveJobRun>(), It.IsAny<IReadOnlyCollection<ArchiveBookLogEntry>>(), It.IsAny<CancellationToken>()))
            .Callback<ArchiveJobRun, IReadOnlyCollection<ArchiveBookLogEntry>, CancellationToken>((run, logs, _) =>
            {
                savedRun = run;
                savedLogs = logs;
            });

        var service = new BookArchiveProcessingService(
            repository.Object,
            new Mock<IBookService>().Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(new DateOnly(2026, 1, 1), 10);

        Assert.Equal(0, result.ProcessedCount);
        Assert.NotNull(savedRun);
        Assert.NotNull(savedLogs);
        Assert.Empty(savedLogs!);
    }

    [Fact]
    public async Task ProcessAsync_WhenCandidatesMixed_AggregatesResultsAndPersistsLogs()
    {
        var archivedCandidate = new ArchiveBookCandidate
        {
            BookId = Guid.NewGuid(),
            BookTitle = "Archived",
            BookStatus = BookStatus.Available
        };
        var skippedCandidate = new ArchiveBookCandidate
        {
            BookId = Guid.NewGuid(),
            BookTitle = "Skipped",
            BookStatus = BookStatus.Borrow
        };
        var failedCandidate = new ArchiveBookCandidate
        {
            BookId = Guid.NewGuid(),
            BookTitle = "Failed",
            BookStatus = BookStatus.Available
        };

        var repository = new Mock<IBookArchiveRepository>();
        repository.Setup(x => x.GetArchiveCandidates(It.IsAny<DateOnly>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([archivedCandidate, skippedCandidate, failedCandidate]);
        repository.Setup(x => x.GetBooksWithActiveBorrow(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<Guid>());
        ArchiveJobRun? savedRun = null;
        IReadOnlyCollection<ArchiveBookLogEntry>? savedLogs = null;
        repository.Setup(x => x.SaveArchiveProcessingResult(It.IsAny<ArchiveJobRun>(), It.IsAny<IReadOnlyCollection<ArchiveBookLogEntry>>(), It.IsAny<CancellationToken>()))
            .Callback<ArchiveJobRun, IReadOnlyCollection<ArchiveBookLogEntry>, CancellationToken>((run, logs, _) =>
            {
                savedRun = run;
                savedLogs = logs;
            });

        var bookService = new Mock<IBookService>();
        bookService.Setup(x => x.ArchiveBook(archivedCandidate.BookId)).ReturnsAsync(new BookArchive { Id = archivedCandidate.BookId });
        bookService.Setup(x => x.ArchiveBook(failedCandidate.BookId)).ThrowsAsync(new InvalidOperationException("archive failure"));

        var service = new BookArchiveProcessingService(
            repository.Object,
            bookService.Object,
            new TestTimeProvider(new DateTime(2026, 5, 26, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.ProcessAsync(new DateOnly(2026, 1, 1), 10);

        Assert.Equal(3, result.ProcessedCount);
        Assert.Equal(1, result.ArchivedCount);
        Assert.Equal(1, result.SkippedCount);
        Assert.Equal(1, result.FailedCount);
        Assert.NotNull(savedRun);
        Assert.Equal(3, savedRun!.ProcessedCount);
        Assert.NotNull(savedLogs);
        Assert.Equal(3, savedLogs!.Count);
    }
}
