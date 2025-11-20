using Microsoft.EntityFrameworkCore;
using MySettings.Api.Models;

namespace MySettings.Api.Data;

/// <summary>
/// Application database context
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();
    public DbSet<GeneralSettings> GeneralSettings => Set<GeneralSettings>();
    public DbSet<BusinessContent> BusinessContent => Set<BusinessContent>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SiteSettings configuration
        modelBuilder.Entity<SiteSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // GeneralSettings configuration
        modelBuilder.Entity<GeneralSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SiteName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Tagline).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Timezone).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DateFormat).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // BusinessContent configuration
        modelBuilder.Entity<BusinessContent>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.DisplayOrder);
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.DisplayOrder);
            entity.HasIndex(e => e.IsFeatured);
        });

        // QuoteRequest configuration
        modelBuilder.Entity<QuoteRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
