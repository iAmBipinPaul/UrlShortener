using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence;

namespace UrlShortener.Migrator
{
    public class DairyDbContextFactory : DesignTimeDbContextFactoryBase<UrlShortenerDbContext>
    {
        protected override UrlShortenerDbContext CreateNewInstance(DbContextOptions<UrlShortenerDbContext> options)
        {
            return new UrlShortenerDbContext(options);
        }
    }
}
