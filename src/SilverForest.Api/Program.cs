using SilverForest.Api;

await WebApplication
    .CreateBuilder(args)
    .AddApiServices()
    .Build()
    .ConfigureApplication()
    .RunAsync();