using JetBrains.Annotations;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Jobs.Archive;
using PracticalWork.Library.Jobs.Common;
using PracticalWork.Library.Jobs.Notifications;
using PracticalWork.Library.Jobs.Reports;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Controllers;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Options;
using PracticalWork.Library.Web.Configuration;
using System.Text.Json.Serialization;
using PracticalWork.Library.MessageBroker;
using PracticalWork.Library.Services.Email;

namespace PracticalWork.Library.Web;

/// <summary>
/// Конфигурация DI и HTTP pipeline для сервиса библиотеки.
/// </summary>
public class Startup
{
    private static string _basePath;
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Инициализирует конфигурацию старта сервиса библиотеки.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _basePath = string.IsNullOrWhiteSpace(Configuration["GlobalPrefix"]) ? "" : $"/{Configuration["GlobalPrefix"].Trim('/')}";
    }

    /// <summary>
    /// Регистрирует сервисы приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddPostgreSqlStorage(cfg =>
        {
            var connectionString = Configuration
                .GetSection("App")
                .GetConnectionString(nameof(AppDbContext));
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString)
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(npgsqlDataSource);
        });
        services.AddMvc(opt =>
            {
                opt.Filters.Add<DomainExceptionFilter<AppException>>();
            })
            .AddApi()
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
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Controllers.xml"));
        });

        services.AddOptions<EmailSettings>()
            .Bind(Configuration.GetSection("App:Email"))
            .Validate(settings =>
                    !string.IsNullOrWhiteSpace(settings.SmtpServer)
                    && settings.SmtpPort > 0
                    && !string.IsNullOrWhiteSpace(settings.SenderEmail),
                "Некорректная конфигурация App:Email")
            .ValidateOnStart();

        services.AddOptions<JobSettings>()
            .Bind(Configuration.GetSection("App:Jobs"))
            .ValidateOnStart();

        services.AddOptions<ArchiveSettings>()
            .Bind(Configuration.GetSection("App:Archive"))
            .ValidateOnStart();

        services.AddOptions<EmailTemplateSettings>()
            .Bind(Configuration.GetSection("App:EmailTemplates"))
            .ValidateOnStart();

        var appDbConnectionString = Configuration
            .GetSection("App")
            .GetConnectionString(nameof(AppDbContext));

        services.AddHangfire(hangfire =>
            hangfire
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(appDbConnectionString)));
        services.AddHangfireServer();

        services.AddScoped<EmailTemplateRenderer>();
        services.AddScoped<IReturnReminderEmailBuilder, ReturnReminderEmailBuilder>();
        services.AddScoped<IWeeklyAdminReportEmailBuilder, WeeklyAdminReportEmailBuilder>();
        services.AddScoped<IEmailService, MailKitEmailService>();
        services.AddScoped<ReturnReminderJob>();
        services.AddScoped<WeeklyAdminReportJob>();
        services.AddScoped<ArchiveOldBooksJob>();
        services.AddSingleton<IJobManagementService, HangfireJobManagementService>();
        services.AddHostedService<HangfireRecurringJobsHostedService>();
        
        services
            .AddMessageBroker(Configuration)
            .AddProducers();
        services.AddDomain();
        services.AddCache(Configuration);
        services.AddMinioFileStorage(Configuration);
    }

    /// <summary>
    /// Настраивает HTTP pipeline приложения.
    /// </summary>
    /// <param name="app">Построитель конвейера приложения.</param>
    /// <param name="env">Окружение веб-хоста.</param>
    /// <param name="lifetime">Жизненный цикл хоста.</param>
    /// <param name="logger">Логгер приложения.</param>
    /// <param name="serviceProvider">Провайдер сервисов приложения.</param>
    [UsedImplicitly]
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
        ILogger logger, IServiceProvider serviceProvider)
    {
        app.UsePathBase(new PathString(_basePath));

        app.UseRouting();
        app.UseHangfireDashboard("/hangfire");

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
