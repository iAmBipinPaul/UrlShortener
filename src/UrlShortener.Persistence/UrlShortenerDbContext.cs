using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;

namespace UrlShortener.Persistence
{
    public sealed class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {


        }
        public UrlShortenerDbContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<ShortUrlClick> ShortUrlClicks { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrlClick>()
                .HasOne(p => p.ShortUrl)
                .WithMany(b => b.ShortUrlClicks)
                .HasForeignKey(p => p.ShortUrlId);
            
            modelBuilder.Entity<ShortUrl>()
                .HasKey(p => p.ShortName);
        }
    }
}
