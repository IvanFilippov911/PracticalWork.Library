using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Кандидат на архивацию книги.
/// </summary>
public sealed class ArchiveBookCandidate
{
    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid BookId { get; init; }

    /// <summary>
    /// Название книги.
    /// </summary>
    public required string BookTitle { get; init; }

    /// <summary>
    /// Текущий статус книги.
    /// </summary>
    public BookStatus BookStatus { get; init; }

    /// <summary>
    /// Дата последней выдачи.
    /// </summary>
    public DateOnly? LastBorrowDate { get; init; }

    /// <summary>
    /// Есть ли активная выдача на момент выборки.
    /// </summary>
    public bool HasActiveBorrow { get; init; }
}
