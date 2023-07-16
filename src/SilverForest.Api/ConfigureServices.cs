using Serilog;
using SilverForest.Infrastructure;
using HealthChecks.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SilverForest.Api.Services;
using System.Text.Json.Serialization;
using SilverForest.Api.Abstraction.Interfaces;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.AspNetCore.Hosting;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SilverForest.Api;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {   
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app is null) throw new ArgumentNullException(nameof(app));

        app.UseSerilogRequestLogging(options =>
        {
            // Exclude endpoint logging for HealthCheck endpoints
            options.GetLevel = (ctx, elapsed, ex) =>
            {
                if (ctx.Request.Path.StartsWithSegments("/health"))
                {
                    return LogEventLevel.Verbose;
                }
                return LogEventLevel.Information;
            };
        });

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

        app.UseAuthentication();

        app.UseRouting()
            .UseAuthorization()
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
