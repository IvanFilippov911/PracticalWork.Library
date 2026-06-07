namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие выдачи книги читателю.
/// </summary>
/// <param name="BookId">Идентификатор книги.</param>
/// <param name="ReaderId">Идентификатор читателя.</param>
/// <param name="BookTitle">Название книги.</param>
/// <param name="ReaderName">ФИО читателя.</param>
/// <param name="BorrowDate">Дата выдачи.</param>
/// <param name="DueDate">Плановая дата возврата.</param>
public sealed record BookBorrowedEvent(
    Guid BookId,
    Guid ReaderId,
    string BookTitle,
    string ReaderName,
    DateOnly BorrowDate,
    DateOnly DueDate
) : BaseLibraryEvent("book.borrowed");
