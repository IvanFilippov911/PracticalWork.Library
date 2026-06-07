namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие закрытия читательской карточки.
/// </summary>
/// <param name="ReaderId">Идентификатор читателя.</param>
/// <param name="FullName">ФИО читателя.</param>
/// <param name="ClosedAt">Дата и время закрытия карточки.</param>
/// <param name="Reason">Причина закрытия.</param>
public sealed record ReaderClosedEvent(
    Guid ReaderId,
    string FullName,
    DateTime ClosedAt,
    string Reason
) : BaseLibraryEvent("reader.closed");
