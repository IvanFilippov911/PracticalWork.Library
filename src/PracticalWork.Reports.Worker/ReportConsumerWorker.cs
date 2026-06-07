using EasyNetQ;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.MessageBroker.Options;
using PracticalWork.Library.MessageBroker.Utils;

namespace PracticalWork.Reports.Worker;

/// <summary>
/// Фоновый consumer, подписанный на события создания отчетов в RabbitMQ.
/// </summary>
public sealed class ReportConsumerWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqOptions _options;
    private readonly RabbitMqInfrastructureInitializer _initializer;
    private readonly ILogger<ReportConsumerWorker> _logger;

    private IDisposable? _subscription;

    /// <summary>
    /// Инициализирует фоновый consumer отчетов.
    /// </summary>
    /// <param name="bus">Шина сообщений.</param>
    /// <param name="serviceProvider">Провайдер сервисов приложения.</param>
    /// <param name="options">Настройки RabbitMQ.</param>
    /// <param name="initializer">Инициализатор инфраструктуры RabbitMQ.</param>
    /// <param name="logger">Логгер worker-а.</param>
    public ReportConsumerWorker(
        IBus bus,
        IServiceProvider serviceProvider,
        IOptions<RabbitMqOptions> options,
        RabbitMqInfrastructureInitializer initializer,
        ILogger<ReportConsumerWorker> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _initializer = initializer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _initializer.InitializeAsync();

        _subscription = _bus.PubSub.Subscribe<ReportCreateEvent>(
            "report_create_consumer",
            (message, ct) => HandleMessageAsync(message, ct),
            cfg => cfg.WithQueueName(_options.Reports.QueueName),
            stoppingToken);
    }

    /// <summary>
    /// Останавливает подписку на очередь отчетов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены остановки.</param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        _subscription = null;
        await base.StopAsync(cancellationToken);
    }

    private async Task HandleMessageAsync(ReportCreateEvent message, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<ReportCreateEvent>>();
            await handler.HandleAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки сообщения {MessageType}", nameof(ReportCreateEvent));
            throw;
        }
    }
}
