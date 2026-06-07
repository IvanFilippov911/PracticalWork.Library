using PracticalWork.Library.Abstractions.Storage;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Лог архивации книг.
/// </summary>
public sealed class ArchiveLogEntity : EntityBase
{
    /// <summary>
    /// Идентификатор запуска задачи.
    /// </summary>
    public Guid JobRunId { get; set; }

    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid BookId { get; set; }

    /// <summary>
    /// Название книги.
    /// </summary>
    public string BookTitle { get; set; }

    /// <summary>
    /// Результат обработки (archived/skipped/failed/summary).
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Причина или сообщение об ошибке.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Время обработки.
    /// </summary>
    public DateTime ProcessedAt { get; set; }
}
