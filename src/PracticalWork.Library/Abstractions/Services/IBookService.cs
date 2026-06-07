using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с книгами в библиотеке
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Создание книги
    /// </summary>
    /// <returns>Идентификатор созданной книги</returns>
    Task<Guid> CreateBook(Book book);

    /// <summary>
    /// Редактирование книги
    /// </summary>
    Task UpdateBook(Book model, Guid id);
    
    /// <summary>
    /// Перевести книгу в архив
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    Task<BookArchive> ArchiveBook(Guid bookId);
    
    /// <summary>
    /// Получить список книг
    /// </summary>
    /// <returns></returns>
    Task<PagedResult<Book>> GetBooks(BookSearchCriteria requestModel);
    
    /// <summary>
    /// Обновить детали книги (описание и обложка)
    /// </summary>
    Task UpdateBookDetails(Guid id, string description, BookCoverUpload coverFile);

}
