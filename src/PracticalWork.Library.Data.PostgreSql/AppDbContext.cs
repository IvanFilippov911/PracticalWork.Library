using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// EF Core контекст для доменных сущностей сервиса библиотеки.
/// </summary>
public sealed class AppDbContext : DbContext
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Инициализирует контекст данных библиотеки.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста.</param>
    /// <param name="timeProvider">Поставщик времени для обновления служебных полей.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider timeProvider) : base(options)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Применяет конфигурации сущностей из текущей сборки.
    /// </summary>
    /// <param name="builder">Построитель модели EF Core.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    #region Set UpdateDate on SaveChanges

    /// <summary>
    /// Сохраняет изменения и обновляет служебные поля модифицированных сущностей.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Нужно ли подтверждать все изменения после успешного сохранения.</param>
    /// <returns>Количество затронутых строк.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdateDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Асинхронно сохраняет изменения и обновляет служебные поля модифицированных сущностей.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Нужно ли подтверждать все изменения после успешного сохранения.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Количество затронутых строк.</returns>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetUpdateDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetUpdateDates()
    {
        var updateDate = _timeProvider.GetUtcNow().UtcDateTime;

        var updatedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEntries)
        {
            if (entry.Entity is IEntity entity)
                entity.UpdatedAt = updateDate;
        }
    }

    #endregion

    /// <summary>
    /// Набор сущностей книг (базовый тип)
    /// </summary>
    internal DbSet<AbstractBookEntity> Books { get; set; }

    /// <summary>
    /// Набор сущностей учебных книг
    /// </summary>
    internal DbSet<EducationalBookEntity> EducationalBooks { get; set; }

    /// <summary>
    /// Набор сущностей художественных книг
    /// </summary>
    internal DbSet<FictionBookEntity> FictionBooks { get; set; }

    /// <summary>
    /// Набор сущностей научных книг
    /// </summary>
    internal DbSet<ScientificBookEntity> ScientificBooks { get; set; }

    /// <summary>
    /// Набор сущностей читателей
    /// </summary>
    internal DbSet<ReaderEntity> Readers { get; set; }

    /// <summary>
    /// Набор сущностей выдач книг
    /// </summary>
    internal DbSet<BookBorrowEntity> BookBorrows { get; set; }

    /// <summary>
    /// Набор сущностей логов уведомлений
    /// </summary>
    internal DbSet<NotificationLogEntity> NotificationLogs { get; set; }

    /// <summary>
    /// Набор сущностей логов архивации
    /// </summary>
    internal DbSet<ArchiveLogEntity> ArchiveLogs { get; set; }

    /// <summary>
    /// Набор сущностей запусков архивации.
    /// </summary>
    internal DbSet<ArchiveJobRunEntity> ArchiveJobRuns { get; set; }

    /// <summary>
    /// Набор сущностей метаданных еженедельных отчетов
    /// </summary>
    internal DbSet<WeeklyReportMetadataEntity> WeeklyReportMetadata { get; set; }
}
