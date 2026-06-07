namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Базовый рекорд для всех событий в системе.
/// </summary>
public abstract record BaseEvent(
    Guid EventId,
    DateTime OccurredOn,
    string EventType,
    string Source)
{
    protected BaseEvent(Guid eventId, string eventType, string source)
        : this(eventId, DateTime.UtcNow, eventType, source)
    {
    }
}
