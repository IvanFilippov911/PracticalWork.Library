namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Сводка запуска архивации старых книг.
/// </summary>
public sealed class ArchiveJobRun
{
    /// <summary>
    /// Идентификатор запуска.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Пороговая дата последней выдачи.
    /// </summary>
    public DateOnly ThresholdDate { get; init; }

    /// <summary>
    /// Ограничение на количество книг за запуск.
    /// </summary>
    public int MaxBooksPerRun { get; init; }

    /// <summary>
    /// Количество обработанных книг.
    /// </summary>
    public int ProcessedCount { get; init; }

    /// <summary>
    /// Количество успешно архивированных книг.
    /// </summary>
    public int ArchivedCount { get; init; }

    /// <summary>
    /// Количество пропущенных книг.
    /// </summary>
    public int SkippedCount { get; init; }

    /// <summary>
    /// Количество книг с ошибкой обработки.
    /// </summary>
    public int FailedCount { get; init; }

    /// <summary>
    /// Время начала запуска.
    /// </summary>
    public DateTime StartedAt { get; init; }

    /// <summary>
    /// Время завершения запуска.
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Длительность обработки в миллисекундах.
    /// </summary>
    public long DurationMs { get; init; }
}
