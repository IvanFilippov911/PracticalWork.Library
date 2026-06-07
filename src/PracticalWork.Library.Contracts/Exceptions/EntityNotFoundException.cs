namespace PracticalWork.Library.Contracts.Exceptions;

/// <summary>
/// Исключение о том, что сущность не найдена.
/// </summary>
public class EntityNotFoundException<TEntity> : NotFoundException
{
    public EntityNotFoundException(Guid id)
        : base($"Не найдены {typeof(TEntity).FullName} с идентификатором: {id}.")
    {
    }
}
