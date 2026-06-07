using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Запускает применение EF Core миграций для базы данных библиотеки.
/// </summary>
public static class MigrationsRunner
{
    /// <summary>
    /// Применяет ожидающие миграции в контексте приложения.
    /// </summary>
    /// <param name="logger">Логгер для вывода статуса миграции.</param>
    /// <param name="serviceProvider">Провайдер сервисов приложения.</param>
    /// <param name="appName">Имя приложения для префикса в логах.</param>
    public static async Task ApplyMigrations(ILogger logger, IServiceProvider serviceProvider, string appName)
    {
        var operationId = Guid.NewGuid().ToString()[..4];
        logger.LogInformation($"{appName}:UpdateDatabase:{operationId}: starting...");
        try
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            logger.LogInformation($"{appName}:UpdateDatabase:{operationId}: successfully done");
            await Task.FromResult(true);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, $"{appName}:UpdateDatabase.{operationId}: Migration failed");
            throw;
        }
    }
}
