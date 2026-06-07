namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие архивации книги.
/// </summary>
/// <param name="BookId">Идентификатор книги.</param>
/// <param name="Title">Название книги.</param>
/// <param name="Reason">Причина архивации.</param>
/// <param name="ArchivedAt">Дата и время архивации.</param>
public sealed record BookArchivedEvent(
    Guid BookId,
    string Title,
    string Reason,
    DateTime ArchivedAt
) : BaseLibraryEvent("book.archived");
