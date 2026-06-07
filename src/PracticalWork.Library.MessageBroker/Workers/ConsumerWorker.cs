using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.MessageBroker.Options;
using PracticalWork.Library.MessageBroker.Utils;

namespace PracticalWork.Library.MessageBroker.Workers;

/// <summary>
/// Фоновый consumer RabbitMQ для обработки событий библиотеки.
/// </summary>
public class ConsumerWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqOptions _options;
    private readonly RabbitMqInfrastructureInitializer _initializer;
    private readonly ILogger<ConsumerWorker> _logger;

    private readonly List<IDisposable> _subscriptions = new();

    /// <summary>
    /// Инициализирует consumer worker.
    /// </summary>
    /// <param name="bus">Шина EasyNetQ.</param>
    /// <param name="serviceProvider">Провайдер сервисов для создания scoped-обработчиков.</param>
    /// <param name="options">Настройки RabbitMQ.</param>
    /// <param name="initializer">Инициализатор очередей и привязок.</param>
    /// <param name="logger">Логгер фонового сервиса.</param>
    public ConsumerWorker(
        IBus bus,
        IServiceProvider serviceProvider,
        IOptions<RabbitMqOptions> options,
        RabbitMqInfrastructureInitializer initializer,
        ILogger<ConsumerWorker> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _initializer = initializer;
        _logger = logger;
    }

    /// <summary>
    /// Запускает подписки на очереди RabbitMQ и делегирует обработку scoped-хендлерам.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки сервиса.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _initializer.InitializeAsync();

        Subscribe<BookCreatedEvent>(
            "book_create_consumer",
            _options.Library.BookCreate.QueueName,
            stoppingToken);

        Subscribe<BookArchivedEvent>(
            "book_archive_consumer",
            _options.Library.BookArchive.QueueName,
            stoppingToken);

        Subscribe<BookBorrowedEvent>(
            "book_borrow_consumer",
            _options.Library.BookBorrow.QueueName,
            stoppingToken);

        Subscribe<BookReturnedEvent>(
            "book_return_consumer",
            _options.Library.BookReturn.QueueName,
            stoppingToken);

        Subscribe<ReaderCreatedEvent>(
            "reader_create_consumer",
            _options.Library.ReaderCreate.QueueName,
            stoppingToken);

        Subscribe<ReaderClosedEvent>(
            "reader_close_consumer",
            _options.Library.ReaderClose.QueueName,
            stoppingToken);

    }

    /// <summary>
    /// Освобождает активные подписки при остановке фонового сервиса.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции остановки.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
        await base.StopAsync(cancellationToken);
    }

    private void Subscribe<T>(
        string subscriptionId,
        string queueName,
        CancellationToken stoppingToken)
    {
        var subscription = _bus.PubSub.Subscribe<T>(
            subscriptionId,
            (message, ct) => HandleMessageAsync(message, ct),
            cfg => cfg.WithQueueName(queueName),
            stoppingToken);

        _subscriptions.Add(subscription);
    }

    private async Task HandleMessageAsync<T>(
        T message,
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<IMessageHandler<T>>();

            await handler.HandleAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка обработки сообщения {MessageType}",
                typeof(T).Name);

            throw;
        }
    }
}
