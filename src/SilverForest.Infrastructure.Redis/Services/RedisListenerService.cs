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
        _muxer.GetSubscriber().Subscribe("__keyspace@0__:JobQueue:*", async (channel, message) =>
        {
            if (message.ToString() == "zadd")
            {
                _logger.LogInformation($"SUB: Channel: {channel}");
                var (_, key) = channel.ToString().Split(":", 2) switch
                {
                    [var ns, var k] => (ns, k),
                    _ => throw new ArgumentException("Fatal Error when trying to deconstruct channel name!")
                };

                var jobEntry = await _db.SortedSetRangeByScoreWithScoresAsync(key, 1);
                if (jobEntry.Length > 1)
                {
                    _logger.LogInformation("Has jobs queued in: " + key.Split(":", 2)[1]);
                }
                else
                {
                    var id = key.Split(":", 2)[1];
                    _logger.LogInformation($"No jobs queued for ID {id}, will add expiry to newest");

                    var expiryKey = await _db.StringSetAsync($"JobExpiry:{id}", "", expiry: TimeSpan.FromSeconds(3));
                }
            }            
        });
        _muxer.GetSubscriber().Subscribe("__keyevent@0__:expired", async (channel, message) =>
        {
            _logger.LogInformation($"SUB: Channel: {channel}, Message: {message}");

            if (channel.ToString().EndsWith(":expired"))
            {
                var latestJob = await _db.SortedSetPopAsync($"JobQueue:{message.ToString().Split(":", 2)[1]}");
                _logger.LogInformation($"Here I would do something with the data: {latestJob.Value.Element}");

                var setEntries = await _db.SortedSetRangeByScoreWithScoresAsync($"JobQueue:{message.ToString().Split(":", 2)[1]}", 1);
                if (setEntries.Length < 1)
                {
                    _logger.LogInformation("No more jobs queued for this user");
                }
                else
                {
                    _logger.LogInformation($"Found more jobs, queueing next..");
                    var expiryKey = await _db.StringSetAsync($"JobExpiry:{message.ToString().Split(":", 2)[1]}", "", expiry: TimeSpan.FromSeconds(3));
                }
            } else
            {
               _logger.LogInformation($"Unrelated expire event ??????");
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
