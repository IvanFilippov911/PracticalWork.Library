using PracticalWork.Library.Contracts.Abstractions.Services;
using StackExchange.Redis;

namespace PracticalWork.Library.Cache.Redis.Services
{
    /// <inheritdoc />
    public class CacheVersionService : ICacheVersionService
    {
        private readonly IConnectionMultiplexer _redis;

        public CacheVersionService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        /// <inheritdoc />
        public async Task<int> GetVersionAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            return value.IsNullOrEmpty ? 1 : (int)value;
        }

        /// <inheritdoc />
        public async Task<int> IncrementVersionAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            return (int)await db.StringIncrementAsync(key);
        }
    }

}
