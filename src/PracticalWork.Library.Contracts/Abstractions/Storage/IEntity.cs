using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Contracts.Abstractions.Storage;

/// <summary>
/// Базовый контракт сущности хранения.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Идентификатор сущности.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания сущности.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления сущности.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}
