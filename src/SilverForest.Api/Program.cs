using Serilog;
using SilverForest.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDefaults(args)
    .UseSerilog((ctx, config) =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
    });

builder.AddApiServices();

var app = builder.Build();

app.ConfigureApplication();

await app.RunAsync();