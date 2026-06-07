using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.MessageBroker.Handlers;
using PracticalWork.Library.MessageBroker.Options;
using PracticalWork.Library.MessageBroker.Utils;
using PracticalWork.Library.MessageBroker.Workers;

namespace PracticalWork.Library.MessageBroker;

/// <summary>
/// Регистрация компонентов RabbitMQ и фоновых воркеров брокера сообщений.
/// </summary>
public static class Entry
{
    /// <summary>
    /// Регистрирует инфраструктуру RabbitMQ и обработчики сообщений библиотеки.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddMessageBroker(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<RabbitMqOptions>(
            configuration.GetSection("App:RabbitMQ"));
        serviceCollection.AddSingleton<IBus>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            var connectionString = BuildConnectionString(options);
                
            return RabbitHutch.CreateBus(connectionString);
        });
        serviceCollection.AddSingleton<RabbitMqInfrastructureInitializer>();
        
        serviceCollection.AddScoped<IMessageHandler<BookCreatedEvent>, LibraryEventHandler>();
        serviceCollection.AddScoped<IMessageHandler<BookArchivedEvent>, LibraryEventHandler>();
        serviceCollection.AddScoped<IMessageHandler<BookBorrowedEvent>, LibraryEventHandler>();
        serviceCollection.AddScoped<IMessageHandler<BookReturnedEvent>, LibraryEventHandler>();
        serviceCollection.AddScoped<IMessageHandler<ReaderClosedEvent>, LibraryEventHandler>();
        serviceCollection.AddScoped<IMessageHandler<ReaderCreatedEvent>, LibraryEventHandler>();

        return serviceCollection;
    }
    
    /// <summary>
    /// Регистрирует продюсер сообщений и служебный hosted service.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddProducers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IMessageProducer, Producer>();
        serviceCollection.AddHostedService<ProducerWorker>();
        return serviceCollection;
    }
    
    /// <summary>
    /// Регистрирует consumer worker для обработки входящих событий.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddConsumers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<ConsumerWorker>();
        return serviceCollection;
    }
    
    private static string BuildConnectionString(RabbitMqOptions options)
    {
        var builder = new System.Text.StringBuilder();
        builder.Append($"host={options.Host}");
            
        if (options.Port.HasValue)
            builder.Append($":{options.Port}");
                
        builder.Append($";username={options.User}");
        builder.Append($";password={options.Password}");
                
        return builder.ToString();
    }
}
