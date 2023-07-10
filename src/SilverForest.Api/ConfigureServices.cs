using Serilog;
using SilverForest.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace SilverForest.Api;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) 
            => configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddInfrastructureServices(builder.Configuration);

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
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

        return app;
    }
}
