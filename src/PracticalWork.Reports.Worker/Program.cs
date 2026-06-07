using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Library.Contracts.Abstractions.MessageBroker;
using PracticalWork.Library.Contracts.Events;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.Reports.PostgreSql;
using PracticalWork.Library.MessageBroker;
using PracticalWork.Reports.Worker;
using PracticalWork.Reports.Worker.Abstractions;
using PracticalWork.Reports.Worker.Handlers;
using PracticalWork.Reports.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
    .AddMessageBroker(builder.Configuration)
    .AddConsumers();

builder.Services.AddMinioFileStorage(builder.Configuration);
builder.Services.AddCache(builder.Configuration);

builder.Services.AddReportsPostgreSqlStorage(cfg =>
{
    var connectionString = builder.Configuration
        .GetSection("App")
        .GetConnectionString(nameof(ReportsDbContext));
    var npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString)
        .EnableDynamicJson()
        .Build();

    cfg.UseNpgsql(npgsqlDataSource);
});

builder.Services.AddScoped<IReportGenerateService, ReportGenerateService>();
builder.Services.AddScoped<IReportGenerationService, ReportGenerationService>();
builder.Services.AddScoped<IMessageHandler<ReportCreateEvent>, ReportEventHandler>();
builder.Services.AddHostedService<ReportConsumerWorker>();

var host = builder.Build();
host.Run();
