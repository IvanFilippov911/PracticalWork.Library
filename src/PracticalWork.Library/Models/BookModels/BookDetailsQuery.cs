namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Критерий поиска деталей книги по идентификатору или названию.
/// </summary>
public sealed class BookDetailsQuery
{
    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid? BookId { get; init; }

    /// <summary>
    /// Название книги.
    /// </summary>
    public string Title { get; init; }
}
