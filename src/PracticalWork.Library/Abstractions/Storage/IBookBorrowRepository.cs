using PracticalWork.Library.Models.BookModels;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookBorrowRepository
{
    /// <summary>
    /// Создать запись о выдаче
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="readerId">Идентификатор карточки</param>
    /// <param name="bookBorrow">Объект выдачи книги</param>
    /// <returns></returns>
    Task CreateBookBorrow(Guid bookId, Guid readerId, BookBorrow bookBorrow);
    
    /// <summary>
    /// Получить запись о выдаче
    /// </summary>
    /// <param name="bookId">Идентификатор книги</param>
    /// <param name="readerId">Идентификатор карточки</param>
    /// <returns>Идентификатор выдачи и объект выдачи</returns>
    Task<(Guid id, BookBorrow bookBorrow)> GetBookBorrow(Guid bookId, Guid readerId);
    
    /// <summary>
    /// Обновить возвращенную книгу
    /// </summary>
    /// <param name="bookBorrowId">Идентификатор выдачи</param>
    /// <param name="bookBorrow">Объект выдачи</param>
    /// <returns></returns>
    Task UpdateReturnedBookBorrow(Guid bookBorrowId, BookBorrow bookBorrow);
}