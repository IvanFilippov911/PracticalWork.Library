namespace PracticalWork.Library.Contracts.Abstractions.Services;

/// <summary>
/// Сервис для взаимодействия с кешем.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получает значение из кэша по ключу.
    /// </summary>
    /// <typeparam name="T">Тип кэшируемого значения.</typeparam>
    /// <param name="key">Ключ записи в кэше.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Значение из кэша или значение по умолчанию для типа.</returns>
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет значение в кэш на заданное время жизни.
    /// </summary>
    /// <typeparam name="T">Тип кэшируемого значения.</typeparam>
    /// <param name="key">Ключ записи в кэше.</param>
    /// <param name="value">Сохраняемое значение.</param>
    /// <param name="ttl">Время жизни записи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет значение из кэша по ключу.
    /// </summary>
    /// <param name="key">Ключ записи в кэше.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
