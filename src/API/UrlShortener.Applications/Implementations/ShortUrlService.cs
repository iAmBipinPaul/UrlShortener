using LinqKit;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly UrlShortenerContext _urlShortenerContext;
        public ShortUrlService(
            UrlShortenerContext urlShortenerContext
        )
        {
            _urlShortenerContext = urlShortenerContext;
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

            var baseQuery = _urlShortenerContext.ShortUrls.Where(predicate);
            var queryWithLimit = _urlShortenerContext.ShortUrls
                .Where(predicate)
                .OrderByDescending(c => c.LastUpdateDateTime)
                .Skip(req.SkipCount)
                .Take(req.MaxResultCount);
            return new GetShortUrlsResponse()
            {
                TotalCount = await baseQuery.CountAsync(ct),
                Items = await queryWithLimit.Select(c => new ShortUrlsResponse
                {
                    Id = c.Id,
                    ShortName = c.ShortName,
                    DestinationUrl = c.DestinationUrl,
                    LastUpdateDateTime = c.LastUpdateDateTime,
                    CreationDateTime = c.CreationDateTime,
                }).ToListAsync(ct)
            };
        }

        public async Task<CreateOrUpdateShortUrlResponse> CreateShortUrl(CreateOrUpdateShortUrlRequest req,
            CancellationToken ct)
        {
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var partitionValue = req.ShortName.FirstOrDefault().ToString().ToLower();
            var shortUrl = new ShortUrl()
            {
                ShortName = req.ShortName,
                Id = req.ShortName.ToLower(),
                DestinationUrl = req.DestinationUrl,
                LastUpdateDateTime = unixTimeMilliseconds,
                CreationDateTime = unixTimeMilliseconds,
                PartitionValue = partitionValue
            };
            _urlShortenerContext.ShortUrls.Add(shortUrl);
            await _urlShortenerContext.SaveChangesAsync(ct);
            return new CreateOrUpdateShortUrlResponse()
            {
                Id = shortUrl.Id,
                ShortName = req.ShortName,
                DestinationUrl = req.DestinationUrl,
                LastUpdateDateTime = unixTimeMilliseconds,
                CreationDateTime = unixTimeMilliseconds,
            };
        }

        public async Task<ShortUrlsResponse?> GetShortUrl(string shortName, CancellationToken ct)
        {
            var res = await _urlShortenerContext.ShortUrls.Where(c => c.Id== shortName.ToLower())
                .FirstOrDefaultAsync(ct);
            if (res != null)
            {
                return new ShortUrlsResponse()
                {
                    ShortName = res.ShortName,
                    DestinationUrl = res.DestinationUrl,
                    LastUpdateDateTime = res.LastUpdateDateTime,
                    CreationDateTime = res.CreationDateTime,
                };
            }
            return null;
        }

        public async Task DeleteShortUrl(DeleteShortUrlRequest req, CancellationToken ct)
        {
            var user = await _urlShortenerContext.ShortUrls.FirstOrDefaultAsync(
                c => c.Id == req.ShortName.ToLower(), ct);
            if (user != null)
            {
                _urlShortenerContext.ShortUrls.Remove(user);
                await _urlShortenerContext.SaveChangesAsync(ct);
            }
        }

        public async Task<CreateOrUpdateShortUrlResponse?> UpdateShortUrl(CreateOrUpdateShortUrlRequest req,
            CancellationToken ct)
        {
            var shortUrl =
                await _urlShortenerContext.ShortUrls.FirstOrDefaultAsync(c => c.Id == req.ShortName.ToLower(),ct);
            if (shortUrl != null)
            {
                shortUrl.DestinationUrl = req.DestinationUrl;
                shortUrl.LastUpdateDateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _urlShortenerContext.ShortUrls.Update(shortUrl);
                await _urlShortenerContext.SaveChangesAsync(ct);
                return new CreateOrUpdateShortUrlResponse()
                {
                    ShortName = shortUrl.ShortName,
                    CreationDateTime = shortUrl.CreationDateTime,
                    LastUpdateDateTime = shortUrl.LastUpdateDateTime,
                    DestinationUrl = shortUrl.DestinationUrl
                };
            }
            return null;
        }
    }
}