namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие создания читательской карточки.
/// </summary>
/// <param name="ReaderId">Идентификатор читателя.</param>
/// <param name="FullName">ФИО читателя.</param>
/// <param name="PhoneNumber">Телефон читателя.</param>
/// <param name="ExpiryDate">Дата окончания действия карточки.</param>
/// <param name="CreatedAt">Дата и время создания карточки.</param>
public sealed record ReaderCreatedEvent(
    Guid ReaderId,
    string FullName,
    string PhoneNumber,
    DateOnly ExpiryDate,
    DateTime CreatedAt
) : BaseLibraryEvent("reader.created");
