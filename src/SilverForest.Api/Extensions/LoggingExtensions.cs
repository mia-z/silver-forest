using Serilog;

namespace SilverForest.Api.Extensions;
public static class LoggingExtensions
{
    public static WebApplicationBuilder UseSerilog(
        this WebApplicationBuilder builder, 
        Action<WebApplicationBuilder, LoggerConfiguration> configureLogger) 
    {
        var loggerConfig = new LoggerConfiguration();
        configureLogger(builder, loggerConfig);

        var logger = loggerConfig.CreateLogger();
        builder.Services.AddLogging(b => b.ClearProviders());
        builder.Logging.AddSerilog(logger);

        return builder;
    }
}
