    namespace GrpcTestResultService.Caching
{
    public class NullCacheSevice(ILogger<NullCacheSevice> logger) : ICacheService
    {
        
        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("NullCacheService: GetAsync для ключа {Key}", key);
            return Task.FromResult(default(T))!;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            return await factory();
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("NullCacheService: RemoveAsync для ключа {Key}", key);
            return Task.CompletedTask;
        }
     

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("NullCacheService: SetAsync для ключа {Key}", key);
            return Task.CompletedTask;
        }
    }
}
