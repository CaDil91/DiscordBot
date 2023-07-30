using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SteamServices.MySteamDatabase;
public sealed class SteamNewsDatabaseContext : DbContext
{
    private readonly ILogger<SteamNewsDatabaseContext> _logger;
    public DbSet<FollowedSteamApp> FollowedSteamApps { get; set; }
    public DbSet<Webhook> Webhooks { get; set; }

    public SteamNewsDatabaseContext(DbContextOptions<SteamNewsDatabaseContext> options, 
        ILogger<SteamNewsDatabaseContext> logger) : base(options)
    {
        FollowedSteamApps = Set<FollowedSteamApp>();
        Webhooks = Set<Webhook>();
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FollowedSteamApp>()
            .HasKey(followedSteamApp => followedSteamApp.AppId);
        
        modelBuilder.Entity<Webhook>()
            .HasKey(webhook => webhook.WebhookUrl);

        modelBuilder.HasDefaultSchema("SteamNews");
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = Environment.GetEnvironmentVariable("SteamNewsDbConnectionString");
        if (connectionString != null)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            _logger.LogError("SteamNewsDbConnectionString is null");
        }
    }
}