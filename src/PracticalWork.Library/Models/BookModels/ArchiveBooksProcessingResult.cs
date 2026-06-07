namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Результат запуска архивации старых книг.
/// </summary>
public sealed class ArchiveBooksProcessingResult
{
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
    /// Длительность обработки.
    /// </summary>
    public TimeSpan Duration { get; init; }
}
