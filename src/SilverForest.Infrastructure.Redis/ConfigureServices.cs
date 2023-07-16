using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SilverForest.Infrastructure.Redis.Abstraction;
using SilverForest.Infrastructure.Redis.Services;
using StackExchange.Redis;

namespace SilverForest.Infrastructure.Redis;
public static class ConfigureServices
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation("Configuring Infrastructure.Redis Services");

        logger.LogInformation("Reading Redis config vars");
        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");
    
        ArgumentNullException.ThrowIfNull(redisConnectionString, nameof(redisConnectionString));

        logger.LogInformation("Creating Redis mux provider");
        services.AddSingleton<IConnectionMultiplexer>(config =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        logger.LogInformation("Adding RedisListenerService to Container");
        services.AddHostedService<RedisListenerService>();


        logger.LogInformation("Adding Infrastructure.Redis services to Container");
        services.AddScoped<IRedisJobCache, RedisJobCache>();

        logger.LogInformation("Registering Redis Health Checks");
        services.AddHealthChecks()
            .AddRedis(redisConnectionString, "Redis Health Check");

        return services;
    }
}
