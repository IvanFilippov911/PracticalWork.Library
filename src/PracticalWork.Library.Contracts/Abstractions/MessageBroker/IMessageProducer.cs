using PracticalWork.Library.Contracts.Events;

namespace PracticalWork.Library.Contracts.Abstractions.MessageBroker;

/// <summary>
/// Интерфейс продюсера сообщений для публикации событий библиотеки и отчетов.
/// </summary>
public interface IMessageProducer
{
    /// <summary>
    /// Публикует событие создания книги.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceBookCreateAsync(BookCreatedEvent message);

    /// <summary>
    /// Публикует событие архивации книги.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceBookArchiveAsync(BookArchivedEvent message);

    /// <summary>
    /// Публикует событие выдачи книги.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceBookBorrowAsync(BookBorrowedEvent message);

    /// <summary>
    /// Публикует событие возврата книги.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceBookReturnAsync(BookReturnedEvent message);

    /// <summary>
    /// Публикует событие создания читателя.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceReaderCreateAsync(ReaderCreatedEvent message);

    /// <summary>
    /// Публикует событие закрытия читательской карточки.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceReaderCloseAsync(ReaderClosedEvent message);

    /// <summary>
    /// Публикует событие создания отчета.
    /// </summary>
    /// <param name="message">Данные события.</param>
    Task ProduceReportCreateAsync(ReportCreateEvent message);
}
