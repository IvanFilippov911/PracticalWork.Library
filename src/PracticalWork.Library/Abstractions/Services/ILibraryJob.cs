namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Базовый контракт фоновой задачи библиотеки.
/// </summary>
public interface ILibraryJob
{
    /// <summary>Уникальное имя задачи.</summary>
    string JobName { get; }

    /// <summary>Описание задачи.</summary>
    string Description { get; }

    /// <summary>Выполнить задачу.</summary>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
