using GrpcTestResultService.Caching;
using StackExchange.Redis;

namespace GrpcTestResultService.Extensions
{
    public static class RedisServiceExtensions
    {
        public static IServiceCollection AddRedisCashing(this IServiceCollection services,IConfiguration configuration) 
        {
            var redisConfig = configuration.GetSection("Redis");
            if (redisConfig.GetValue<bool>("Enabled", true)){
                services.AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = redisConfig.GetValue<string>("ConnectionString");
                    opt.InstanceName = redisConfig.GetValue<string>("InstanceName", "TestResultService");
                });
                var connectionString = redisConfig.GetValue<string>("ConnectionString");
                var redisConnection = ConnectionMultiplexer.Connect(connectionString!);
                services.AddSingleton<IConnectionMultiplexer>(redisConnection);
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddScoped<ICacheService, NullCacheSevice>();
            }
            return services;
        }

    }
}
