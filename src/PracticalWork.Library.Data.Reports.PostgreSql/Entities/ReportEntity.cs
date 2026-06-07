using PracticalWork.Library.Contracts.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Data.Reports.PostgreSql.Entities;

/// <summary>
/// Сущность отчета
/// </summary>
public class ReportEntity : EntityBase
{
    /// <summary>
    /// Наименование отчета
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Путь к файлу отчета
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Дата и время формирования отчета
    /// </summary>
    public DateTime? GeneratedAt { get; set; }

    /// <summary>
    /// Начало отчетного периода
    /// </summary>
    public DateOnly? PeriodFrom { get; set; }

    /// <summary>
    /// Окончание отчетного периода
    /// </summary>
    public DateOnly? PeriodTo { get; set; }

    /// <summary>
    /// Текущий статус отчета
    /// </summary>
    public ReportStatus Status { get; set; }

    /// <summary>
    /// Типы событий, включенные в отчет
    /// </summary>
    public IReadOnlyList<string>? EventTypes { get; set; }
}
