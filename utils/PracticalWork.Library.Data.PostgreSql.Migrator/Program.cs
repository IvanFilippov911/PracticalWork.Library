using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Data.Reports.PostgreSql;

namespace PracticalWork.Library.Data.PostgreSql.Migrator;

/// <summary>
/// Консольное приложение для применения миграций баз данных библиотеки и отчетов.
/// </summary>
[UsedImplicitly]
public class Program
{
    private const string AppName = "PracticalWork.Library.Data.PostgreSql.Migrator";

    private static IConfiguration Configuration { get; set; }

    private static readonly ILogger SystemLogger = CreateSystemLogger();

    /// <summary>
    /// Запускает применение миграций для `AppDbContext` и `ReportsDbContext`.
    /// </summary>
    public static async Task Main()
    {
        try
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            await MigrateDatabase();
        }
        catch (Exception exception)
        {
            SystemLogger.LogCritical(exception, "Critical error in Main");
            throw;
        }
    }

    private static async Task MigrateDatabase()
    {
        var serviceProvider = CreateServices();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        await MigrationsRunner.ApplyMigrations(logger, serviceProvider, AppName);
        await ReportsMigrationsRunner.ApplyMigrations(logger, serviceProvider, AppName);
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        return services
            .AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.ClearProviders();
            })
            .AddSingleton(TimeProvider.System)
            .AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration["App:AppDbContext"],
                sqlServerOptions => sqlServerOptions.CommandTimeout(Configuration.GetValue<int>("App:MigrationTimeoutInSeconds"))))
            .AddDbContext<ReportsDbContext>(options => options.UseNpgsql(Configuration["App:ReportsDbContext"],
                sqlServerOptions => sqlServerOptions.CommandTimeout(Configuration.GetValue<int>("App:MigrationTimeoutInSeconds"))))
            .BuildServiceProvider(false);
    }

    private static ILogger CreateSystemLogger()
    {
        var logger = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            })
            .BuildServiceProvider()
            .GetRequiredService<ILogger<Program>>();

        return logger;
    }
}
