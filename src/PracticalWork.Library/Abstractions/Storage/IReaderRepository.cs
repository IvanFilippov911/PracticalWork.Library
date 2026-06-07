using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.ReaderModels;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IReaderRepository
{
    /// <summary>
    /// Создать карточку
    /// </summary>
    /// <param name="reader">Объект карточки</param>
    /// <returns>Идентификатор карточки</returns>
    Task<Guid> CreateReader(Reader reader);
    
    /// <summary>
    /// Проверить существование карточки
    /// </summary>
    /// <param name="phone">Телефон читателя</param>
    /// <returns>Истина или ложь</returns>
    Task<bool> IsExistReader(string phone);
    
    /// <summary>
    /// Получить карточку читателя
    /// </summary>
    /// <param name="id">Идентификатор карточки</param>
    /// <returns>Объект карточки</returns>
    Task<Reader> GetReader(Guid id);
    
    /// <summary>
    /// Обновить карточку читателя
    /// </summary>
    /// <param name="id">Идентификатор карточки</param>
    /// <param name="reader">Объект карточки</param>
    /// <returns></returns>
    Task UpdateReader(Guid id, Reader reader);
    
    /// <summary>
    /// Получить информацию о карточке вместе с записями выдачи
    /// </summary>
    /// <param name="id">Идентификатор карточки</param>
    /// <returns>Объект карточки</returns>
    Task<Reader> GetReaderWithBorrowBooks(Guid id);
    
    /// <summary>
    /// Получить выданные читателю книги
    /// </summary>
    /// <param name="id">Идентификатор карточки</param>
    /// <returns>Список выданных книг</returns>
    Task<IReadOnlyList<BorrowedBook>> GetReadersBorrowBooks(Guid id);
}