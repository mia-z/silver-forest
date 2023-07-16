using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Threading.Channels;

namespace SilverForest.Infrastructure.Redis.Services;

public class RedisListenerService : IHostedService, IDisposable
{
    private readonly ILogger<RedisListenerService> _logger;
    private readonly IConnectionMultiplexer _muxer;
    private readonly IDatabase _db;

    public RedisListenerService(IConnectionMultiplexer multiplexer, ILogger<RedisListenerService> logger)
    {
        _logger = logger;
        _muxer = multiplexer;
        _db = multiplexer.GetDatabase();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting RedisListener Service");

        _logger.LogInformation($"Creating JobQueue Subscriber");
        _muxer.GetSubscriber().Subscribe(new RedisChannel("__keyspace@0__:JobQueue:*", RedisChannel.PatternMode.Auto), async (channel, message) =>
        {
            if (message.ToString() == "zadd")
            {
                var (_, key) = channel.ToString().Split(":", 2) switch
                {
                    [var ns, var k] => (ns, k),
                    _ => throw new ArgumentException("Fatal Error when trying to deconstruct channel name!")
                };

                var jobEntry = await _db.SortedSetRangeByScoreWithScoresAsync(key, 1);
                if (jobEntry.Length == 1)
                {
                    var id = key.Split(":", 2)[1];
                    var expiryKey = await _db.StringSetAsync($"JobExpiry:{id}", "", expiry: TimeSpan.FromSeconds(3));
                }
            }            
        });

        _logger.LogInformation($"Creating JobQueue Expiry Subscriber");
        _muxer.GetSubscriber().Subscribe(new RedisChannel("__keyevent@0__:expired", RedisChannel.PatternMode.Auto), async (channel, message) =>
        {
            if (channel.ToString().EndsWith(":expired"))
            {
                var latestJob = await _db.SortedSetPopAsync($"JobQueue:{message.ToString().Split(":", 2)[1]}");
                //TODO: Do something with the data in latest job since it's been popped

                var setEntries = await _db.SortedSetRangeByScoreWithScoresAsync($"JobQueue:{message.ToString().Split(":", 2)[1]}", 1);
                if (setEntries.Length > 0)
                {
                    var expiryKey = await _db.StringSetAsync($"JobExpiry:{message.ToString().Split(":", 2)[1]}", "", expiry: TimeSpan.FromSeconds(3));
                }
            }           
        });

        _logger.LogInformation($"Started RedisListener Service");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _muxer?.Close();
        _muxer?.Dispose();
    }
}
