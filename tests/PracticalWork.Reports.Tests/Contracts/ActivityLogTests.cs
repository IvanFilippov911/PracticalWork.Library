using System.Text.Json;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Contracts.Models.ReportModels;

namespace PracticalWork.Reports.Tests.Contracts;

public sealed class ActivityLogTests
{
    [Fact]
    public void SerializeEvent_WhenEventAssigned_ReturnsJsonPayload()
    {
        var activityLog = new ActivityLog
        {
            EventType = "book.created",
            EventDate = new DateTime(2026, 5, 26, 12, 0, 0, DateTimeKind.Utc),
            Event = new BookCreatedEvent(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Clean Architecture",
                "ScientificBook",
                ["Robert Martin"],
                2017)
        };

        var json = activityLog.SerializeEvent();

        Assert.Contains("book.created", json);
        Assert.Contains("library-service", json);
    }

    [Theory]
    [MemberData(nameof(DeserializeCases))]
    public void DeserializeEvent_WhenKnownType_ReturnsExpectedEventType(string eventType, string json, Type expectedType)
    {
        var result = ActivityLog.DeserializeEvent(eventType, json);

        Assert.NotNull(result);
        Assert.IsType(expectedType, result);
    }

    [Fact]
    public void DeserializeEvent_WhenUnknownType_ThrowsJsonException()
    {
        var action = () => ActivityLog.DeserializeEvent("unknown.event", "{}");

        Assert.Throws<JsonException>(action);
    }

    public static TheoryData<string, string, Type> DeserializeCases()
    {
        return new TheoryData<string, string, Type>
        {
            { "book.created", JsonSerializer.Serialize(new BookCreatedEvent(Guid.NewGuid(), "Book", "ScientificBook", ["Author"], 2026)), typeof(BookCreatedEvent) },
            { "book.archived", JsonSerializer.Serialize(new BookArchivedEvent(Guid.NewGuid(), "Book", "Reason", DateTime.UtcNow)), typeof(BookArchivedEvent) },
            { "book.borrowed", JsonSerializer.Serialize(new BookBorrowedEvent(Guid.NewGuid(), Guid.NewGuid(), "Book", "Reader", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 10))), typeof(BookBorrowedEvent) },
            { "book.returned", JsonSerializer.Serialize(new BookReturnedEvent(Guid.NewGuid(), Guid.NewGuid(), "Book", "Reader", new DateOnly(2026, 5, 12))), typeof(BookReturnedEvent) },
            { "reader.created", JsonSerializer.Serialize(new ReaderCreatedEvent(Guid.NewGuid(), "Reader", "+79990000000", new DateOnly(2027, 1, 1), DateTime.UtcNow)), typeof(ReaderCreatedEvent) },
            { "reader.closed", JsonSerializer.Serialize(new ReaderClosedEvent(Guid.NewGuid(), "Reader", DateTime.UtcNow, "Expired")), typeof(ReaderClosedEvent) }
        };
    }
}
