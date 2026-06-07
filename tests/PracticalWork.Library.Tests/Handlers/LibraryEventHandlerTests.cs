using Moq;
using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Models.ReportModels;
using PracticalWork.Library.MessageBroker.Handlers;

namespace PracticalWork.Library.Tests.Handlers;

public sealed class LibraryEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventReceived_SavesMappedActivityLog()
    {
        var repository = new Mock<IActivityLogRepository>();
        ActivityLog? savedLog = null;
        repository.Setup(x => x.AddLogAsync(It.IsAny<ActivityLog>()))
            .Callback<ActivityLog>(log => savedLog = log)
            .Returns(Task.CompletedTask);
        var handler = new LibraryEventHandler(repository.Object);
        var libraryEvent = new BookCreatedEvent(
            Guid.NewGuid(),
            "Domain-Driven Design",
            "ScientificBook",
            ["Eric Evans"],
            2003)
        {
            OccurredOn = new DateTime(2026, 5, 26, 12, 30, 0, DateTimeKind.Utc)
        };

        await handler.HandleAsync(libraryEvent, CancellationToken.None);

        repository.Verify(x => x.AddLogAsync(It.IsAny<ActivityLog>()), Times.Once);
        Assert.NotNull(savedLog);
        Assert.Same(libraryEvent, savedLog!.Event);
        Assert.Equal("book.created", savedLog.EventType);
        Assert.Equal(libraryEvent.OccurredOn, savedLog.EventDate);
    }
}
