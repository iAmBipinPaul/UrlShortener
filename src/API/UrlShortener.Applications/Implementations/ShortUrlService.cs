using LinqKit;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly Container _containerClient;
        private readonly ILogger<ShortUrlService> _logger;
        private readonly bool _debugMode;
        private readonly UrlShortenerContext _urlShortenerContext;

        public ShortUrlService(ICosmosDbClient cosmosDbClient,
            UrlShortenerContext urlShortenerContext,
            IConfiguration configuration,
            ILogger<ShortUrlService> logger
        )
        {
            _urlShortenerContext = urlShortenerContext;
            _debugMode = configuration.GetValue<bool>("DebugMode");
            _logger = logger;
            _containerClient = cosmosDbClient.GetContainerClient();
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
                TotalCount = await CosmosLinqExtensions.CountAsync(baseQuery, ct),
                Items = await queryWithLimit.Select(c => new ShortUrlsResponse
                {
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
            var tasks = new List<Task>()
            {
                _containerClient.CreateItemAsync(shortUrl, cancellationToken: ct),
                _urlShortenerContext.SaveChangesAsync(ct)
            };
            await Task.WhenAll(tasks);
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
            var partitionValue = req.ShortName.FirstOrDefault().ToString().ToLower();
            var tasks = new List<Task>
            {
                _containerClient.DeleteItemAsync<ShortUrl>(req.ShortName, new PartitionKey(partitionValue),
                    cancellationToken: ct)
            };
            var user = await _urlShortenerContext.ShortUrls.FirstOrDefaultAsync(
                c => c.Id == req.ShortName.ToLower(), ct);
            if (user != null)
            {
                _urlShortenerContext.ShortUrls.Remove(user);
                tasks.Add(_urlShortenerContext.SaveChangesAsync(ct));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<CreateOrUpdateShortUrlResponse?> UpdateShortUrl(CreateOrUpdateShortUrlRequest req,
            CancellationToken ct)
        {
            var partitionValue = req.ShortName.FirstOrDefault().ToString().ToLower();
            var queryable = _containerClient
                .GetItemLinqQueryable<ShortUrl>()
                .Where(c => c.PartitionValue == partitionValue && c.ShortName.ToLower() == req.ShortName.ToLower());

            var linqQuery = queryable;

            var shortUrls = await
                CosmosSqlHelper<ShortUrl>.ToListAsync(_containerClient, linqQuery, _logger,
                    _debugMode);

            if (shortUrls.Any())
            {
                var shortUrl = new ShortUrl() { Id = shortUrls[0].ShortName.ToLower() };
                _urlShortenerContext.ShortUrls.Update(shortUrl);
                shortUrl.DestinationUrl = req.DestinationUrl;
                shortUrl.LastUpdateDateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var tasks = new List<Task> { _urlShortenerContext.SaveChangesAsync(ct) };
                List<PatchOperation> patchOperations = new List<PatchOperation>
                {
                    PatchOperation.Set($"/DestinationUrl", req.DestinationUrl),
                    PatchOperation.Set($"/LastUpdateDateTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                };
                tasks.Add(_containerClient.PatchItemAsync<ShortUrl>(
                    id: shortUrls[0].ShortName,
                    partitionKey: new PartitionKey(partitionValue),
                    patchOperations: patchOperations, cancellationToken: ct));
                await Task.WhenAll(tasks);
                return new CreateOrUpdateShortUrlResponse()
                {
                    ShortName = shortUrls[0].ShortName,
                    CreationDateTime = shortUrls[0].CreationDateTime,
                    LastUpdateDateTime = shortUrl.LastUpdateDateTime,
                    DestinationUrl = shortUrl.DestinationUrl
                };
            }

            return null;
        }
    }
}