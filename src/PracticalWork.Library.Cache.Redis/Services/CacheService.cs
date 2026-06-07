using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PracticalWork.Library.Contracts.Abstractions.Services;

namespace PracticalWork.Library.Cache.Redis.Services
{
    /// <inheritdoc />
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await _cache.GetAsync(key, cancellationToken);

            if (data is null)
                return default;

            return JsonSerializer.Deserialize<T>(data, _jsonOptions);
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan ttl,
            CancellationToken cancellationToken = default)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);

            await _cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
