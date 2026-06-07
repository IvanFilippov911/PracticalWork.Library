using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models.BookModels;
using PracticalWork.Library.Models.ReaderModels;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис сценариев работы с читательскими карточками.
/// </summary>
public class ReaderService: IReaderService
{
    private readonly IReaderRepository _readerRepository;
    private readonly IMessageProducer _producer;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Инициализирует сервис сценариев работы с читателями.
    /// </summary>
    /// <param name="repository">Репозиторий читателей.</param>
    /// <param name="producer">Продюсер сообщений.</param>
    /// <param name="timeProvider">Поставщик времени.</param>
    public ReaderService(IReaderRepository repository,
        IMessageProducer producer,
        TimeProvider timeProvider)
    {
        _readerRepository = repository;
        _producer = producer;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Создает карточку читателя.
    /// </summary>
    /// <param name="reader">Модель читателя.</param>
    /// <returns>Идентификатор созданного читателя.</returns>
    public async Task<Guid> CreateReader(Reader reader)
    {
        if (await _readerRepository.IsExistReader(reader.PhoneNumber))
        {
            throw new ReaderServiceException("Phone number is not unique");
        }
        reader.IsActive = true;
        var id = await _readerRepository.CreateReader(reader);
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        var message = new ReaderCreatedEvent(id, reader.FullName,
            reader.PhoneNumber, reader.ExpiryDate, utcNow);
        await _producer.ProduceReaderCreateAsync(message);
        return id;
    }

    /// <summary>
    /// Продлевает срок действия читательской карточки.
    /// </summary>
    /// <param name="id">Идентификатор читателя.</param>
    /// <param name="date">Новая дата окончания действия.</param>
    public async Task ExtendExpiryDate(Guid id, DateOnly date)
    {
        var reader = await _readerRepository.GetReader(id);
        if (!reader.IsActive)
        {
            throw new ReaderServiceException("Карточка неактивна");
        }

        if (reader.ExpiryDate >= date)
        {
            throw new ReaderServiceException("Необходимо продлить карточку на будущую дату");
        }
        reader.ExpiryDate = date;
        await _readerRepository.UpdateReader(id, reader);
    }

    /// <summary>
    /// Закрывает карточку читателя, если за ним не числятся книги.
    /// </summary>
    /// <param name="id">Идентификатор читателя.</param>
    /// <returns>Признак наличия книг и список этих книг.</returns>
    public async Task<(bool borrowBooksExist, IReadOnlyList<Book> borrowBooks)> CloseReader(Guid id)
    {
        var readerWithBorrowBooks = await _readerRepository.GetReaderWithBorrowBooks(id);
        var borrowBooksExist = readerWithBorrowBooks.BorrowBooks.Any();
        if (borrowBooksExist)
        {
            return (true, readerWithBorrowBooks.BorrowBooks);
        }
        readerWithBorrowBooks.IsActive = false;
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        readerWithBorrowBooks.ExpiryDate = DateOnly.FromDateTime(utcNow);
        await _readerRepository.UpdateReader(id, readerWithBorrowBooks);
        var message = new ReaderClosedEvent(id, readerWithBorrowBooks.FullName,
            utcNow, "Вызван метод закрытия карточки");
        await _producer.ProduceReaderCloseAsync(message);
        return (false, readerWithBorrowBooks.BorrowBooks);
    }

    /// <summary>
    /// Возвращает список всех книг, выданных читателю.
    /// </summary>
    /// <param name="readerId">Идентификатор читателя.</param>
    /// <returns>Список выданных книг.</returns>
    public async Task<IReadOnlyList<BorrowedBook>> GetAllBorrowBooks(Guid readerId)
    {
        var result = await _readerRepository
            .GetReadersBorrowBooks(readerId);
        return result;
    }
}
