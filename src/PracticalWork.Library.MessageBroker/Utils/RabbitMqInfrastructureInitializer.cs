using EasyNetQ;
using EasyNetQ.Logging;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using PracticalWork.Library.MessageBroker.Options;

namespace PracticalWork.Library.MessageBroker.Utils;

/// <summary>
/// Инициализатор инфраструктуры RabbitMQ: создает обмены, очереди и привязки согласно настройкам приложения.
/// </summary>
public class RabbitMqInfrastructureInitializer
{
    private readonly IAdvancedBus _advancedBus;
    private readonly RabbitMqOptions _options;
    
    /// <summary>
    /// Конструктор класса инициализации RabbitMQ
    /// </summary>
    /// <param name="bus">Экземпляр шины сообщений EasyNetQ</param>
    /// <param name="options">Настройки подключения и конфигурации RabbitMQ</param>
    public RabbitMqInfrastructureInitializer(
        IBus bus,
        IOptions<RabbitMqOptions> options)
    {
        _advancedBus = bus.Advanced;
        _options = options.Value;
    }
    
    /// <summary>
    /// Асинхронно инициализирует все обмены, очереди и привязки для библиотеки и отчетов
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию</returns>
    public async Task InitializeAsync()
    {
        var libraryExchange = await _advancedBus.ExchangeDeclareAsync(
            _options.Library.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false);
        
        await InitializeLibraryQueuesAndBindings(libraryExchange);
        
        var reportExchange = await _advancedBus.ExchangeDeclareAsync(
            _options.Reports.Exchange,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false);
        
        await InitializeReportsQueueAndBinding(reportExchange);
    }
    
    /// <summary>
    /// Асинхронно инициализирует очереди библиотеки и связывает их с указанным обменом
    /// </summary>
    /// <param name="exchange">Обмен RabbitMQ для библиотеки</param>
    /// <returns>Задача, представляющая асинхронную операцию</returns>
    private async Task InitializeLibraryQueuesAndBindings(Exchange exchange)
    {
        var bookCreateQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.BookCreate.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, bookCreateQueue, _options.Library.BookCreate.RoutingKey);
        
        var bookArchiveQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.BookArchive.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, bookArchiveQueue, _options.Library.BookArchive.RoutingKey);

        var bookBorrowQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.BookBorrow.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, bookBorrowQueue, _options.Library.BookBorrow.RoutingKey);
        
        var bookReturnQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.BookReturn.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, bookReturnQueue, _options.Library.BookReturn.RoutingKey);
        
        var readerCreateQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.ReaderCreate.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, readerCreateQueue, _options.Library.ReaderCreate.RoutingKey);
        
        var readerCloseQueue = await _advancedBus.QueueDeclareAsync(
            _options.Library.ReaderClose.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, readerCloseQueue, _options.Library.ReaderClose.RoutingKey);
    }
    
    /// <summary>
    /// Асинхронно инициализирует очередь отчетов и связывает её с указанным обменом
    /// </summary>
    /// <param name="exchange">Обмен RabbitMQ для отчетов</param>
    /// <returns>Задача, представляющая асинхронную операцию</returns>
    private async Task InitializeReportsQueueAndBinding(Exchange exchange)
    {
        var reportsQueue = await _advancedBus.QueueDeclareAsync(
            _options.Reports.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        await _advancedBus.BindAsync(exchange, reportsQueue, _options.Reports.RoutingKey);
    }
}