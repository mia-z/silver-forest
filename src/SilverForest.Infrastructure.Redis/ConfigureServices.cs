using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SilverForest.Infrastructure.Redis.Services;
using StackExchange.Redis;

namespace SilverForest.Infrastructure.Redis;
public static class ConfigureServices
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

        ArgumentNullException.ThrowIfNull(redisConnectionString, nameof(redisConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(config =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.AddScoped<RedisService>();

        services.AddHealthChecks()
            .AddRedis(redisConnectionString, "Redis Health Check");

        return services;
    }
}
