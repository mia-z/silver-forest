using SilverForest.Api;
using Microsoft.EntityFrameworkCore.Design;

await WebApplication
    .CreateBuilder(args)
    .AddApiServices()
    .Build()
    .ConfigureApplication()
    .RunAsync();