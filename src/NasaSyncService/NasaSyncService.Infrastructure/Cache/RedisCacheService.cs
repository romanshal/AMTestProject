using Microsoft.Extensions.Options;
using NasaSyncService.Application.Interfaces.Cache;
using NasaSyncService.Infrastructure.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace NasaSyncService.Infrastructure.Cache
{
    /// <summary>
    /// Service for redis cache.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="options"></param>
    internal class RedisCacheService(
        IConnectionMultiplexer connection,
        IOptions<RedisSettings> options) : ICacheService
    {
        private readonly IDatabase _db = connection.GetDatabase();
        private readonly RedisSettings _settings = options.Value;

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var expiry = TimeSpan.FromSeconds(_settings.ExpirationTimeSeconds);
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
