using PracticalWork.Library.Contracts.Abstractions.Services;
using PracticalWork.Library.Contracts.Options;

namespace PracticalWork.Library.Contracts.Helpers;

/// <summary>
/// Набор вспомогательных методов для работы с версионируемым кэшем.
/// </summary>
public static class CacheManager
{
    /// <summary>
    /// Инвалидирует все кэши, связанные с книгами.
    /// </summary>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="cacheOptions">Опции кэша для книг.</param>
    public static async Task InvalidateBookCacheAsync(ICacheVersionService cacheVersionService, BooksCacheOptions cacheOptions)
    {
        if (cacheOptions?.BooksListCacheOptions?.Prefix != null)
        {
            await cacheVersionService.IncrementVersionAsync(cacheOptions.BooksListCacheOptions.Prefix);
        }

        if (cacheOptions?.LibraryBooksCacheOptions?.Prefix != null)
        {
            await cacheVersionService.IncrementVersionAsync(cacheOptions.LibraryBooksCacheOptions.Prefix);
        }

        if (cacheOptions?.BookDetailsCacheOptions?.Prefix != null)
        {
            await cacheVersionService.IncrementVersionAsync(cacheOptions.BookDetailsCacheOptions.Prefix);
        }
    }

    /// <summary>
    /// Инвалидирует кэш списка отчетов.
    /// </summary>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="cacheOptions">Опции кэша.</param>
    public static async Task InvalidateReportsCacheAsync(ICacheVersionService cacheVersionService, BooksCacheOptions cacheOptions)
    {
        if (cacheOptions?.ReportsCacheOptions?.Prefix != null)
        {
            await cacheVersionService.IncrementVersionAsync(cacheOptions.ReportsCacheOptions.Prefix);
        }
    }

    /// <summary>
    /// Пытается получить коллекцию моделей из кэша и преобразовать их в целевой тип.
    /// </summary>
    /// <typeparam name="TModel">Целевой тип модели.</typeparam>
    /// <typeparam name="TCache">Тип модели, лежащей в кэше.</typeparam>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="cacheService">Сервис распределенного кэша.</param>
    /// <param name="prefix">Префикс кэша.</param>
    /// <param name="parameter">Параметры запроса для генерации ключа.</param>
    /// <param name="map">Функция преобразования модели из кэша.</param>
    /// <returns>Коллекция моделей, найденных в кэше.</returns>
    public static async Task<List<TModel>> CheckCacheAsync<TModel, TCache>(
        ICacheVersionService cacheVersionService,
        ICacheService cacheService,
        string prefix,
        object parameter,
        Func<TCache, TModel> map)
    {
        if (prefix == null)
        {
            return [];
        }

        var version = await cacheVersionService.GetVersionAsync(prefix);
        var cacheKey = CacheKeyHasher.GenerateCacheKey(
            prefix,
            version,
            new
            {
                parameter
            });

        var cachedItems = await cacheService.GetAsync<List<TCache>>(cacheKey);
        if (cachedItems == null || cachedItems.Count == 0)
        {
            return [];
        }

        return cachedItems.Select(map).ToList();
    }

    /// <summary>
    /// Сохраняет коллекцию моделей в кэш.
    /// </summary>
    /// <typeparam name="TModel">Исходный тип модели.</typeparam>
    /// <typeparam name="TCache">Тип модели для хранения в кэше.</typeparam>
    /// <param name="cacheVersionService">Сервис версионирования кэша.</param>
    /// <param name="cacheService">Сервис распределенного кэша.</param>
    /// <param name="option">Настройки кэширования.</param>
    /// <param name="parameter">Параметры запроса для генерации ключа.</param>
    /// <param name="modelList">Коллекция моделей для сохранения.</param>
    /// <param name="map">Функция преобразования модели в кэшируемый тип.</param>
    public static async Task WriteToCacheAsync<TModel, TCache>(
        ICacheVersionService cacheVersionService,
        ICacheService cacheService,
        CacheOptions option,
        object parameter,
        IReadOnlyList<TModel> modelList,
        Func<TModel, TCache> map)
    {
        if (option?.Prefix == null)
        {
            return;
        }

        var version = await cacheVersionService.GetVersionAsync(option.Prefix);
        var cacheKey = CacheKeyHasher.GenerateCacheKey(
            option.Prefix,
            version,
            new
            {
                parameter
            });

        var dto = modelList.Select(map).ToList();
        await cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(option.TtlMinutes));
    }
}
