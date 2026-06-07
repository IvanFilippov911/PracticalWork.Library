using PracticalWork.Library.Abstractions.Storage;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Сводка запуска архивации старых книг.
/// </summary>
public sealed class ArchiveJobRunEntity : EntityBase
{
    /// <summary>
    /// Пороговая дата последней выдачи.
    /// </summary>
    public DateOnly ThresholdDate { get; set; }

    /// <summary>
    /// Ограничение на количество книг за запуск.
    /// </summary>
    public int MaxBooksPerRun { get; set; }

    /// <summary>
    /// Количество обработанных книг.
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Количество успешно архивированных книг.
    /// </summary>
    public int ArchivedCount { get; set; }

    /// <summary>
    /// Количество пропущенных книг.
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Количество книг с ошибкой обработки.
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Время начала запуска.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Время завершения запуска.
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Длительность обработки в миллисекундах.
    /// </summary>
    public long DurationMs { get; set; }
}
