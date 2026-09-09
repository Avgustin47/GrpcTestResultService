using StackExchange.Redis;
using UserMqDto;
namespace NotificationService.Services
{
    public class RedisTokenStorage : ITokenStorage
    {
        private readonly IDatabase _db;

        public RedisTokenStorage(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task RemoveTokenAsync(string token)
        {
            var key = $"download_token:{token}";
            await _db.KeyDeleteAsync(key);
        }

        public async Task SaveTokenAsync(string token, int reportId, TimeSpan lifetime)
        {
            var key = $"download_token:{token}";
            await _db.StringSetAsync(key, reportId.ToString(), lifetime);
        }

        public async Task<(bool Exists, int ReportId)> TryGetReportIdAsync(string token)
        {
            var key = $"download_token:{token}";
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return (false, 0);

            // Явно преобразуем в строку
            var stringValue = value.ToString();
            if (int.TryParse(stringValue, out var reportId))
                return (true, reportId);

            return (false, 0);
        }
    }
}
