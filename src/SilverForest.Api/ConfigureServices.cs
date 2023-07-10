using Serilog;
using SilverForest.Infrastructure;
using HealthChecks.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace SilverForest.Api;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) 
            => configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        builder.Services.AddControllers();

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app is null) throw new ArgumentNullException(nameof(app));

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
                endpoints.MapHealthChecksUI();
            });

        return app;
    }
}
