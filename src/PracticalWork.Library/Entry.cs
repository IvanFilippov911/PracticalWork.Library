using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Services;

namespace PracticalWork.Library;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей уровня бизнес-логики
    /// </summary>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IBookArchiveProcessingService, BookArchiveProcessingService>();
        services.AddScoped<IReturnReminderProcessingService, ReturnReminderProcessingService>();
        services.AddScoped<IWeeklyAdminReportCsvBuilder, WeeklyAdminReportCsvBuilder>();
        services.AddScoped<IWeeklyAdminReportPeriodService, WeeklyAdminReportPeriodService>();
        services.AddScoped<IWeeklyAdminReportStorageService, WeeklyAdminReportStorageService>();
        services.AddScoped<IWeeklyAdminReportNotificationService, WeeklyAdminReportNotificationService>();
        services.AddScoped<IWeeklyAdminReportProcessingService, WeeklyAdminReportProcessingService>();
        services.AddScoped<IReaderService, ReaderService>();
        services.AddScoped<ILibraryService, LibraryService>();
        return services;
    }
}
