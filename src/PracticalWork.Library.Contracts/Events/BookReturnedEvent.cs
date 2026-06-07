namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие возврата книги читателем.
/// </summary>
/// <param name="BookId">Идентификатор книги.</param>
/// <param name="ReaderId">Идентификатор читателя.</param>
/// <param name="BookTitle">Название книги.</param>
/// <param name="ReaderName">ФИО читателя.</param>
/// <param name="ReturnDate">Дата возврата.</param>
public sealed record BookReturnedEvent(
    Guid BookId,
    Guid ReaderId,
    string BookTitle,
    string ReaderName,
    DateOnly ReturnDate
) : BaseLibraryEvent("book.returned");
