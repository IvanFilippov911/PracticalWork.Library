using Microsoft.Extensions.Hosting;
using PracticalWork.Library.MessageBroker.Utils;

namespace PracticalWork.Library.MessageBroker.Workers;

/// <summary>
/// Фоновый сервис для инициализации инфраструктуры RabbitMQ при запуске приложения
/// </summary>
public class ProducerWorker : BackgroundService
{
    private readonly RabbitMqInfrastructureInitializer _initializer;
    
    /// <summary>
    /// Конструктор фонового сервиса
    /// </summary>
    /// <param name="initializer">Инициализатор инфраструктуры RabbitMQ</param>
    public ProducerWorker(RabbitMqInfrastructureInitializer initializer)
    {
        _initializer = initializer;
    }
    
    /// <summary>
    /// Асинхронное выполнение фонового сервиса:
    /// инициализация обменов, очередей и привязок RabbitMQ
    /// </summary>
    /// <param name="stoppingToken">Токен отмены фоновой службы</param>
    /// <returns>Задача, представляющая асинхронную операцию</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _initializer.InitializeAsync();
    }
}