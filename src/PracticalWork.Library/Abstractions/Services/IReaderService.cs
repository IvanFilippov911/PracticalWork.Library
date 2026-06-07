using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.ReaderModels;

namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Интерфейс сервиса работы с читателями
    /// </summary>
    public interface IReaderService
    {
        /// <summary>
        /// Создать нового читателя
        /// </summary>
        /// <param name="reader">Объект читателя</param>
        /// <returns>Идентификатор созданного читателя</returns>
        Task<Guid> CreateReader(Reader reader);

        /// <summary>
        /// Продлить срок действия читательского билета
        /// </summary>
        /// <param name="id">Идентификатор читателя</param>
        /// <param name="date">Новая дата окончания действия</param>
        Task ExtendExpiryDate(Guid id, DateOnly date);

        /// <summary>
        /// Закрыть читателя, проверяя наличие выданных книг
        /// </summary>
        /// <param name="id">Идентификатор читателя</param>
        /// <returns>
        /// Кортеж, где <c>borrowBooksExist</c> указывает на наличие выданных книг,
        /// а <c>borrowBooks</c> содержит список этих книг
        /// </returns>
        Task<(bool borrowBooksExist, IReadOnlyList<Book> borrowBooks)> CloseReader(Guid id);

        /// <summary>
        /// Получить список всех книг, выданных конкретному читателю
        /// </summary>
        /// <param name="readerId">Идентификатор читателя</param>
        /// <returns>Список выданных книг</returns>
        Task<IReadOnlyList<BorrowedBook>> GetAllBorrowBooks(Guid readerId);
    }
}