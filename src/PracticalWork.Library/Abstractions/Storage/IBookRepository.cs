using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookRepository
{
    /// <summary>
    /// Создать книгу
    /// </summary>
    /// <param name="book">Модель книги</param>
    /// <returns>Идентификатор созданной книги</returns>
    Task<Guid> CreateBook(Book book);
    
    /// <summary>
    /// Отредактировать книгу
    /// </summary>
    /// <param name="book">Модель для редактирования книги</param>
    /// <param name="bookId">Идентификатор книги</param>
    /// <returns>-</returns>
    Task UpdateBook(Book book, Guid bookId);
    
    /// <summary>
    /// Получить книгу по идентификатору
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <returns>Книга</returns>
    Task<Book> GetBookById(Guid bookId);

    /// <summary>
    /// Получить список книг с фильтрами
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <returns>Список книг</returns>
    Task<(IReadOnlyList<Book>, int)> GetBooks(BookSearchCriteria request);
    /// <summary>
    /// Получить не архивные книги с записями о выдаче
    /// </summary>
    /// <param name="request">Объект пагинации</param>
    /// <returns>Список книг</returns>
    Task<(IReadOnlyList<Book>, int)> GetNonArchivedBooksPageWithIssuanceRecords(
        PageRequest request);
    /// <summary>
    /// Получить книгу по названию
    /// </summary>
    /// <param name="title">Название книги</param>
    /// <returns>Идентификатор книги</returns>
    Task<Guid> GetBookIdByTitle(string title);
}
