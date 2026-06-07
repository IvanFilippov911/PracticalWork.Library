namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Базовый класс для всех сущностей
/// </summary>
public abstract class EntityBase : IEntity
{
    
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания сущности
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления сущности
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Конструктор базовой сущности, инициализирует Id и CreatedAt
    /// </summary>
    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = TimeProvider.System.GetUtcNow().UtcDateTime;
    }
}
