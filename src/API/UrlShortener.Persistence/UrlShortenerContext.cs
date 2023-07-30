using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;

namespace UrlShortener.Persistence;

public class UrlShortenerContext: DbContext
{
    
    public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
        : base(options)
    {


    }
    public UrlShortenerContext() 
    {
        Database.EnsureCreated();
    }
    public DbSet<ShortUrl> ShortUrls { get; set; }
    public DbSet<ShortUrlClick> ShortUrlClicks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ShortUrl>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever();
            entity.Ignore(e => e.EntityKind);
            entity.Ignore(e => e.PartitionValue);
            entity.Ignore(e => e.ttl);
            entity
                .HasMany(p => p.ShortUrlClicks) 
                .WithOne(c => c.ShortUrl) 
                .HasForeignKey(c => c.ShortUrlId);
        });
        builder.Entity<ShortUrlClick>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Ignore(e => e.EntityKind);
            entity.Ignore(e => e.PartitionValue);
            entity.Ignore(e => e.ttl);
        });
        base.OnModelCreating(builder);
    }
}