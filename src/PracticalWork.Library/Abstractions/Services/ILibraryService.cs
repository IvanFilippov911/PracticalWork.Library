using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.Common;

namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Интерфейс сервиса работы с библиотекой
    /// </summary>
    public interface ILibraryService
    {
        /// <summary>
        /// Выдать книгу читателю
        /// </summary>
        /// <param name="bookId">Идентификатор книги</param>
        /// <param name="readerId">Идентификатор читателя</param>
        /// <returns>Задача, представляющая асинхронную операцию</returns>
        Task BorrowBook(Guid bookId, Guid readerId);

        /// <summary>
        /// Вернуть книгу из библиотеки
        /// </summary>
        /// <param name="bookId">Идентификатор книги</param>
        /// <param name="readerId">Идентификатор читателя</param>
        /// <returns>Задача, представляющая асинхронную операцию</returns>
        Task ReturnBook(Guid bookId, Guid readerId);

        /// <summary>
        /// Получить детали книги по критерию поиска.
        /// </summary>
        /// <param name="query">Критерий поиска книги.</param>
        Task<(Guid bookId, Book book)> GetBookDetails(BookDetailsQuery query);

        /// <summary>
        /// Получить детали книги по её идентификатору
        /// </summary>
        /// <param name="bookId">Идентификатор книги</param>
        /// <returns>Кортеж с идентификатором книги и объектом книги</returns>
        Task<(Guid bookId, Book book)> GetBookDetails(Guid bookId);

        /// <summary>
        /// Получить детали книги по её названию
        /// </summary>
        /// <param name="title">Название книги</param>
        /// <returns>Кортеж с идентификатором книги и объектом книги</returns>
        Task<(Guid bookId, Book book)> GetBookDetails(string title);

        /// <summary>
        /// Получить страницу неархивированных книг с пагинацией
        /// </summary>
        /// <param name="request">Параметры пагинации</param>
        /// <returns>Страница с объектами книг</returns>
        Task<PagedResult<Book>> GetNonArchivedBooksPage(PageRequest request);
    }
}
