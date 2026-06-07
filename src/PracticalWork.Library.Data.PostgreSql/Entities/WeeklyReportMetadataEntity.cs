using PracticalWork.Library.Abstractions.Storage;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Метаданные еженедельного отчета библиотеки.
/// </summary>
public sealed class WeeklyReportMetadataEntity : EntityBase
{
    /// <summary>
    /// Имя отчета.
    /// </summary>
    public string ReportName { get; set; }

    /// <summary>
    /// Имя bucket в MinIO.
    /// </summary>
    public string BucketName { get; set; }

    /// <summary>
    /// Имя объекта в bucket.
    /// </summary>
    public string ObjectName { get; set; }

    /// <summary>
    /// Начало периода отчета.
    /// </summary>
    public DateOnly PeriodFrom { get; set; }

    /// <summary>
    /// Конец периода отчета.
    /// </summary>
    public DateOnly PeriodTo { get; set; }

    /// <summary>
    /// Количество новых книг.
    /// </summary>
    public int NewBooksCount { get; set; }

    /// <summary>
    /// Количество новых читателей.
    /// </summary>
    public int NewReadersCount { get; set; }

    /// <summary>
    /// Количество выдач.
    /// </summary>
    public int BorrowedCount { get; set; }

    /// <summary>
    /// Количество возвратов.
    /// </summary>
    public int ReturnedCount { get; set; }

    /// <summary>
    /// Количество просроченных выдач на конец недели.
    /// </summary>
    public int OverdueCount { get; set; }

    /// <summary>
    /// Удален ли отчет по политике хранения.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Время удаления отчета.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
