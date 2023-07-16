using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SilverForest.Infrastructure.Postgres;
using SilverForest.Infrastructure.Redis;

namespace SilverForest.Infrastructure;
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("InfrastructureLogger");

        logger.LogInformation("Starting Infrastructure Services");
        services.AddPostgres(configuration, logger);
        services.AddRedis(configuration, logger);
        
        return services;
    }
}