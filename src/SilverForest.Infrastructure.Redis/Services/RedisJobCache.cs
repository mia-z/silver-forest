using Microsoft.Extensions.Logging;
using SilverForest.Common.Models;
using SilverForest.Infrastructure.Redis.Abstraction;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;

namespace SilverForest.Infrastructure.Redis.Services;

public class RedisJobCache : IRedisJobCache
{
    private readonly ILogger<RedisJobCache> _logger;
    private readonly IDatabase _db;

    public RedisJobCache(ILogger<RedisJobCache> logger, IConnectionMultiplexer muxer)
    {
        _logger = logger;
        _db = muxer.GetDatabase();
    }

    public async Task<bool> AddJob(string name, int id)
    {
        try
        {
            var jobKey = $"JobQueue:{id}";
            await _db.SortedSetAddAsync(jobKey, name, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return true;
        } catch (Exception ex)
        {
            _logger.LogError("Error when trying to create job\n" + ex.Message);
            return false;
        }
    }

    public async Task<IEnumerable<SkillJob>> GetJobsById(int id)
    {
        try
        {
            var jobKey = $"JobQueue:{id}";
            var jobs = await _db.SortedSetRangeByScoreAsync(jobKey);

            var formattedJobs = jobs.ToList()
                .Select(x => JsonSerializer.Deserialize<SkillJob>(x))
                .ToList();

            return formattedJobs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when trying to fetch jobs for id: {id}\n" + ex.Message);
            return await Task.FromResult(Enumerable.Empty<SkillJob>());
        }
    }
}
