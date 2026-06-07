using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.MessageBroker.Options;

namespace PracticalWork.Library.MessageBroker;

/// <summary>
/// Публикует доменные события библиотеки и отчетов в RabbitMQ.
/// </summary>
public class Producer : IMessageProducer
{
    private readonly IAdvancedBus _advancedBus;
    private readonly RabbitMqOptions _options;
    private readonly TimeProvider _timeProvider;
    
    /// <summary>
    /// Инициализирует продюсер сообщений.
    /// </summary>
    /// <param name="bus">Шина EasyNetQ.</param>
    /// <param name="options">Настройки RabbitMQ.</param>
    /// <param name="timeProvider">Поставщик времени для заполнения метаданных сообщения.</param>
    public Producer(
        IBus bus,
        IOptions<RabbitMqOptions> options,
        TimeProvider timeProvider)
    {
        _advancedBus = bus.Advanced;
        _options = options.Value;
        _timeProvider = timeProvider;
    }
    
    /// <inheritdoc />
    public async Task ProduceBookCreateAsync(BookCreatedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.BookCreate.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceBookArchiveAsync(BookArchivedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.BookArchive.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceBookBorrowAsync(BookBorrowedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.BookBorrow.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceBookReturnAsync(BookReturnedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.BookReturn.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceReaderCreateAsync(ReaderCreatedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.ReaderCreate.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceReaderCloseAsync(ReaderClosedEvent message)
    {
        await ProduceToLibraryAsync(_options.Library.ReaderClose.RoutingKey, message);
    }
    
    /// <inheritdoc />
    public async Task ProduceReportCreateAsync(ReportCreateEvent message)
    {
        await ProduceToReportsAsync(_options.Reports.RoutingKey, message);
    }
    
    private async Task ProduceToLibraryAsync<T>(string routingKey, T message) where T : class
    {
        await ProduceAsync(_options.Library.ExchangeName, routingKey, message);
    }
    
    private async Task ProduceToReportsAsync<T>(string routingKey, T message) where T : class
    {
        await ProduceAsync(_options.Reports.Exchange, routingKey, message);
    }
    
    private async Task ProduceAsync<T>(string exchangeName, string routingKey, T message) where T : class
    {
        var exchange = await _advancedBus.ExchangeDeclareAsync(
            exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
        var messageObj = CreateMessage(message);
        await _advancedBus.PublishAsync(exchange, routingKey, false, messageObj);
    }
    
    private IMessage<T> CreateMessage<T>(T body) where T : class
    {
        var message = new Message<T>(body)
        {
            Properties =
            {
                DeliveryMode = 2,
                AppId = _options.AppName,
                Timestamp = _timeProvider.GetUtcNow().ToUnixTimeSeconds()
            }
        };
        
        return message;
    }
}
