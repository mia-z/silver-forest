using StackExchange.Redis;

namespace SilverForest.Infrastructure.Redis.Services;

public class RedisService
{
    private readonly IDatabase _db;
    public RedisService(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }
}
