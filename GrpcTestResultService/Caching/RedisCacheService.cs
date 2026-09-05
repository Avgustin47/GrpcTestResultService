using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using System.Text.Json;

namespace GrpcTestResultService.Caching
{
    public class RedisCacheService(IDistributedCache cache,ILogger <RedisCacheService> logger )  : ICacheService
    {
        private readonly IDistributedCache cache = cache;
        
        private readonly ILogger <RedisCacheService> logger = logger;
        private readonly JsonSerializerOptions jsonoptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };


        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await cache.GetStringAsync(key, cancellationToken);
                return !string.IsNullOrEmpty(data);
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            
            try
            {
                var data = await cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(data))
                {
                    logger.LogWarning("Кэш не найден : {key}", key);
                    return default!;
                }
                var result = JsonSerializer.Deserialize<T>(data, jsonoptions);
                logger.LogInformation("Данные получены из кэша для ключа:{key}", key);
                return result!;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Ошибка при получении из кэша для ключа:{key}", key);
                return default!;
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var cached = await GetAsync<T>(key, cancellationToken);
            if(cached != null)
            {
                return cached;
            }
            var result = await factory();

            if(result != null)
            {
                await SetAsync(key, result, expiry, cancellationToken);
            }
            return result;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await cache.RemoveAsync(key, cancellationToken);
                logger.LogDebug("Ключ удален из кэша: {key}", key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при удалении из кэша для ключа: {key}", key);
            }
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = JsonSerializer.Serialize(value, jsonoptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
                };
                await cache.SetStringAsync(key, data, options, cancellationToken);
                logger.LogDebug("Данные сохранены в кэш для ключа: {key}", key);

            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Ошибка при сохрании в кэш для ключа : {key}", key);
            }
        }
    }
}
