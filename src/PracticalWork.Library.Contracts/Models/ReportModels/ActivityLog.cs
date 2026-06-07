using System.Text.Json;
using PracticalWork.Library.Contracts.Events;

namespace PracticalWork.Library.Contracts.Models.ReportModels;

/// <summary>
/// Запись активности системы.
/// </summary>
public class ActivityLog
{
    public string EventType { get; set; }

    public DateTime EventDate { get; set; }

    public BaseEvent Event { get; set; }

    public string SerializeEvent()
    {
        return JsonSerializer.Serialize(Event);
    }

    public static BaseEvent DeserializeEvent(string eventType, string jsonb)
    {
        return eventType switch
        {
            "book.created" => JsonSerializer.Deserialize<BookCreatedEvent>(jsonb),
            "book.archived" => JsonSerializer.Deserialize<BookArchivedEvent>(jsonb),
            "book.borrowed" => JsonSerializer.Deserialize<BookBorrowedEvent>(jsonb),
            "book.returned" => JsonSerializer.Deserialize<BookReturnedEvent>(jsonb),
            "reader.created" => JsonSerializer.Deserialize<ReaderCreatedEvent>(jsonb),
            "reader.closed" => JsonSerializer.Deserialize<ReaderClosedEvent>(jsonb),
            _ => throw new JsonException("Unknown event")
        };
    }
}
