using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SilverForest.Infrastructure.Postgres;
using SilverForest.Infrastructure.Redis;

namespace SilverForest.Infrastructure;
public static class ConfigurationExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddRedis(configuration);

        return services;
    }
}