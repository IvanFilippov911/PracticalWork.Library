using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Cache.Redis.Services;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Options;
using StackExchange.Redis;

namespace PracticalWork.Library.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей для распределенного Cache
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["App:Redis:RedisCacheConnection"];
        var prefix = configuration["App:Redis:RedisCachePrefix"];

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            serviceCollection.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            serviceCollection.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    options.InstanceName = prefix;
                }
            });
        }

        serviceCollection.AddScoped<ICacheService, CacheService>();
        serviceCollection.AddScoped<ICacheVersionService, CacheVersionService>();

        serviceCollection.Configure<BooksCacheOptions>(configuration.GetSection("App:BooksCache"));

        return serviceCollection;
    }
}
