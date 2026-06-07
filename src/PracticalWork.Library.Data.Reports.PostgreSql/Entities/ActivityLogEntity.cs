using PracticalWork.Library.Contracts.Abstractions.Storage;

namespace PracticalWork.Library.Data.Reports.PostgreSql.Entities;

/// <summary>
/// Сущность журнала активности
/// </summary>
public class ActivityLogEntity : EntityBase
{
    /// <summary>
    /// Идентификатор книги, связанной с событием
    /// </summary>
    public Guid? BookId { get; set; }

    /// <summary>
    /// Идентификатор читателя, связанного с событием
    /// </summary>
    public Guid? ReaderId { get; set; }

    /// <summary>
    /// Тип события
    /// </summary>
    public string EventType { get; set; } = null!;

    /// <summary>
    /// Дата и время возникновения события
    /// </summary>
    public DateTime EventDate { get; set; }

    /// <summary>
    /// Дополнительные данные события в сериализованном виде
    /// </summary>
    public string? Metadata { get; set; }
}
