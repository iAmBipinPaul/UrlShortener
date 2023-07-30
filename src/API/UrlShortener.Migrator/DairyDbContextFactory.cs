using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence;

namespace UrlShortener.Migrator
{
    public class DairyDbContextFactory : DesignTimeDbContextFactoryBase<UrlShortenerContext>
    {
        protected override UrlShortenerContext CreateNewInstance(DbContextOptions<UrlShortenerContext> options)
        {
            return new UrlShortenerContext(options);
        }
    }
}
