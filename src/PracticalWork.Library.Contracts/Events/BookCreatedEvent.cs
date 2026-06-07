namespace PracticalWork.Library.Contracts.Events;

/// <summary>
/// Событие создания новой книги.
/// </summary>
/// <param name="BookId">Идентификатор книги.</param>
/// <param name="Title">Название книги.</param>
/// <param name="Category">Категория книги.</param>
/// <param name="Authors">Авторы книги.</param>
/// <param name="Year">Год издания.</param>
public sealed record BookCreatedEvent(
    Guid BookId,
    string Title,
    string Category,
    string[] Authors,
    int Year
) : BaseLibraryEvent("book.created");
