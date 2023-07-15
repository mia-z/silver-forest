using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SilverForest.Infrastructure.Postgres.Abstraction;
using SilverForest.Infrastructure.Postgres.Services;

namespace SilverForest.Infrastructure.Postgres;
public static class ConfigurationExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresString = configuration.GetConnectionString("PgsqlConnectionString");

        ArgumentNullException.ThrowIfNull(postgresString, nameof(postgresString));

        services.AddDbContext<SilverForestDbContext>(options => 
        {
            options.UseNpgsql(postgresString);
        });

        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddHealthChecks()
            .AddNpgSql(postgresString, name: "Postgres Health Check");

        return services;
    }
}
