namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Модель, представляющая архивированную книгу
/// </summary>
public class BookArchive
{
    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название книги
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Дата архивирования
    /// </summary>
    public DateTime ArchivedAt { get; set; }
}