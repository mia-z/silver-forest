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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User { Id = 0, Name = "Admin", Email = "ryan@miaz.xyz" });

        modelBuilder.Entity<Skill>().HasData(new Skill { Id = 1, Name = "Forestry" } );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserSkillData> SkillData { get; set; }
    public DbSet<Skill> Skills { get; set; }
}