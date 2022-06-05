using LinqKit;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Applications;

public class ShortUrlService : IShortUrlService
{
    private readonly UrlShortenerDbContext _urlShortenerDbContext;

    public ShortUrlService(UrlShortenerDbContext urlShortenerDbContext)
    {
        _urlShortenerDbContext = urlShortenerDbContext;
    }

    public async Task<GetShortUrlsResponse> GetShortUrls(GetShortUrlsRequest req, CancellationToken ct)
    {
        var predicate = PredicateBuilder.New<ShortUrl>(true);

        if (!string.IsNullOrWhiteSpace(req.Query))
        {
            var reqQuery = req.Query;
            predicate = predicate.And(p =>
                p.ShortName.Contains(reqQuery)
                ||
                p.DestinationUrl.Contains(reqQuery)
            );
        }

        var query = _urlShortenerDbContext.ShortUrls
            .Where(predicate);
        var res = await query
            .Skip(req.SkipCount)
            .Take(req.MazResultCount).ToListAsync(ct);

        return new GetShortUrlsResponse()
        {
            TotalCount = await query.CountAsync(ct),
            Items = res.Select(c => new ShortUrlsResponse
            {
                ShortName = c.ShortName,
                DestinationUrl = c.DestinationUrl,
                LastUpdateDateTime = c.LastUpdateDateTime,
                CreationDateTime = c.CreationDateTime,
            }).ToList()
        };
    }

    public async Task<CreateShortUrlResponse> CreateShortUrl(CreateShortUrlRequest req, CancellationToken ct)
    {
        var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _urlShortenerDbContext.ShortUrls.Add(new ShortUrl()
        {
            ShortName = req.ShortName,
            DestinationUrl = req.DestinationUrl,
            LastUpdateDateTime = unixTimeMilliseconds,
            CreationDateTime = unixTimeMilliseconds,
        });
        await _urlShortenerDbContext.SaveChangesAsync(ct);
        return new CreateShortUrlResponse()
        {
            ShortName = req.ShortName,
            DestinationUrl = req.DestinationUrl,
            LastUpdateDateTime = unixTimeMilliseconds,
            CreationDateTime = unixTimeMilliseconds,
        };
    }

    public async Task<ShortUrlsResponse?> GetShortUrl(string shortName, CancellationToken ct)
    {
        var result =
            await _urlShortenerDbContext.ShortUrls.FirstOrDefaultAsync(c => c.ShortName == shortName,
                cancellationToken: ct);
        if (result != null)
        {
            return new ShortUrlsResponse()
            {
                ShortName = result.ShortName,
                DestinationUrl = result.DestinationUrl,
                LastUpdateDateTime = result.LastUpdateDateTime,
                CreationDateTime = result.CreationDateTime,
            };
        }

        return null;
    }
}
