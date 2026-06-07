namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Базовый рекорд для всех событий сервиса библиотеки.
/// </summary>
public abstract record BaseLibraryEvent(string EventType)
    : BaseEvent(Guid.NewGuid(), EventType, "library-service")
{
    protected BaseLibraryEvent() : this(string.Empty)
    {
    }
}
