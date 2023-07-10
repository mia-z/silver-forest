using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SilverForest.Common.Models;

namespace SilverForest.Infrastructure.Postgres.Services;
public class SilverForestDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public SilverForestDbContext(DbContextOptions<SilverForestDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PgsqlConnectionString"));
    }

    public DbSet<User> Users { get; set; }
}