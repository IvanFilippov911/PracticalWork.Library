using Moq;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Reports.Worker.Abstractions;
using PracticalWork.Reports.Worker.Handlers;

namespace PracticalWork.Reports.Tests.Worker;

public sealed class ReportEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenReportCreateEventReceived_ForwardsArgumentsToGenerationService()
    {
        var generationService = new Mock<IReportGenerationService>();
        var handler = new ReportEventHandler(generationService.Object);
        var reportEvent = new ReportCreateEvent(
            Guid.NewGuid(),
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 7),
            ["book.created", "reader.created"],
            ReportStatus.InProgress);

        await handler.HandleAsync(reportEvent, CancellationToken.None);

        generationService.Verify(x => x.GenerateReportAsync(
            reportEvent.Id,
            reportEvent.PeriodFrom,
            reportEvent.PeriodTo,
            It.Is<string[]>(types => types.SequenceEqual(new[] { "book.created", "reader.created" })),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
