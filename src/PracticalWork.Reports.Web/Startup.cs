using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Exceptions;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.Reports.PostgreSql;
using PracticalWork.Library.MessageBroker;
using PracticalWork.Reports.Web.Configuration;
using PracticalWork.Reports.Web.Services;
using System.Text.Json.Serialization;

namespace PracticalWork.Reports.Web;

/// <summary>
/// Конфигурация DI и HTTP pipeline для сервиса отчетов.
/// </summary>
public class Startup
{
    private static string _basePath = string.Empty;
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Инициализирует конфигурацию старта сервиса отчетов.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        var globalPrefix = Configuration["GlobalPrefix"];
        _basePath = string.IsNullOrWhiteSpace(globalPrefix) ? string.Empty : $"/{globalPrefix.Trim('/')}";
    }

    /// <summary>
    /// Регистрирует сервисы приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddReportsPostgreSqlStorage(cfg =>
        {
            var connectionString = Configuration
                .GetSection("App")
                .GetConnectionString(nameof(ReportsDbContext));
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString)
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(npgsqlDataSource);
        });

        services.AddReportsApi()
            .AddMvcOptions(opt =>
            {
                opt.Filters.Add<DomainExceptionFilter<AppException>>();
            })
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        services.AddSwaggerGen(c =>
        {
            c.UseOneOfForPolymorphism();
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Contracts.xml"));
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Reports.Web.xml"));
        });

        services.AddMessageBroker(Configuration).AddProducers();
        services.AddCache(Configuration);
        services.AddMinioFileStorage(Configuration);

        services.AddScoped<IReportService, ReportService>();
    }

    /// <summary>
    /// Настраивает HTTP pipeline приложения.
    /// </summary>
    /// <param name="app">Построитель конвейера приложения.</param>
    /// <param name="env">Окружение веб-хоста.</param>
    /// <param name="lifetime">Жизненный цикл хоста.</param>
    /// <param name="logger">Логгер приложения.</param>
    /// <param name="serviceProvider">Провайдер сервисов приложения.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
        ILogger logger, IServiceProvider serviceProvider)
    {
        app.UsePathBase(new PathString(_basePath));

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var descriptions = endpoints.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });
            endpoints.MapControllers();
        });
    }
}
