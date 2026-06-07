using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Repositories;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Регистрация EF Core контекста и репозиториев PostgreSQL для сервиса библиотеки.
/// </summary>
public static class Entry
{
    private static readonly Action<DbContextOptionsBuilder> DefaultOptionsAction = (_) => { };

    /// <summary>
    /// Добавляет зависимости для работы с PostgreSQL-хранилищем.
    /// </summary>
    /// <param name="serviceCollection">Коллекция сервисов приложения.</param>
    /// <param name="optionsAction">Делегат настройки контекста EF Core.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddPostgreSqlStorage(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> optionsAction)
    {
        serviceCollection.AddDbContext<AppDbContext>(optionsAction ?? DefaultOptionsAction);

        serviceCollection.AddScoped<IBookRepository, BookRepository>();
        serviceCollection.AddScoped<IBookArchiveRepository, BookArchiveRepository>();
        serviceCollection.AddScoped<IReturnReminderRepository, ReturnReminderRepository>();
        serviceCollection.AddScoped<IWeeklyAdminReportRepository, WeeklyAdminReportRepository>();
        serviceCollection.AddScoped<IReaderRepository, ReaderRepository>();
        serviceCollection.AddScoped<IBookBorrowRepository, BookBorrowRepository>();
        
        return serviceCollection;
    }
}
