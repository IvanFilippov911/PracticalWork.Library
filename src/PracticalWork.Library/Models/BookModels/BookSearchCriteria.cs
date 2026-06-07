using PracticalWork.Library.Enums;
using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Критерии поиска книг для application-слоя.
/// </summary>
public sealed class BookSearchCriteria : PageRequest
{
    /// <summary>
    /// Статус книги.
    /// </summary>
    public BookStatus? Status { get; init; }

    /// <summary>
    /// Категория книги.
    /// </summary>
    public BookCategory? Category { get; init; }

    /// <summary>
    /// Автор.
    /// </summary>
    public string Author { get; init; }
}
