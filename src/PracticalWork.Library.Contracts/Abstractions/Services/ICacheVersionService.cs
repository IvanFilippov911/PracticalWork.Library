namespace PracticalWork.Library.Contracts.Abstractions.Services;

/// <summary>
/// Сервис для версионирования кеша.
/// </summary>
public interface ICacheVersionService
{
    /// <summary>
    /// Возвращает текущую версию для пространства кэша.
    /// </summary>
    /// <param name="key">Ключ пространства кэша.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Текущая версия пространства кэша.</returns>
    Task<int> GetVersionAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Увеличивает версию пространства кэша.
    /// </summary>
    /// <param name="key">Ключ пространства кэша.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Новое значение версии.</returns>
    Task<int> IncrementVersionAsync(string key, CancellationToken cancellationToken = default);
}
