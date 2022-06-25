using LinqKit;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlClickService : IShortUrlClickService
    {
        private readonly Container _containerClient;
        private readonly bool _debugMode;
        private readonly ILogger<ShortUrlClickService> _logger;

        public ShortUrlClickService(ICosmosDbClient cosmosDbClient,IConfiguration configuration,ILogger<ShortUrlClickService> logger)
        {
            _debugMode = configuration.GetValue<bool>("DebugMode");
            _logger = logger;
            _containerClient = cosmosDbClient.GetContainerClient();
        }
        public async Task InsertShortUrlClick(InsertShortUrlClickInput input, CancellationToken ct)
        {
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var partitionValue = input.ShortUrlId.FirstOrDefault().ToString().ToLower();
            await _containerClient.CreateItemAsync(new ShortUrlClick()
            {
                ShortUrlId = input.ShortUrlId,
                CreationDateTime = unixTimeMilliseconds,
                IpAddress = input.IpAddress,
                ClientInfo = input.ClientInfo, 
                PartitionValue = partitionValue,
                IpInfo = input.IpInfo
            }, cancellationToken: ct);
        }

        public async Task<GetShortUrlClicksResponse> GetShortUrlClicks(GetShortUrlClicksRequest req, CancellationToken ct)
        {
            var predicate = PredicateBuilder.New<ShortUrlClick>(true);
            var partitionValue = req.ShortUrlId.FirstOrDefault().ToString().ToLower();
            predicate = predicate.And(p =>
                p.ShortUrlId==req.ShortUrlId && p.EntityKind==EntityKind.ShortUrlClick
            );
            predicate = predicate.And(p => p.PartitionValue == partitionValue);
            if (!string.IsNullOrWhiteSpace(req.Query))
            {
                var reqQuery = req.Query;
            }
            
            var queryable = _containerClient
                .GetItemLinqQueryable<ShortUrlClick>()
                .Where(predicate)
                .OrderByDescending(c => c.CreationDateTime);

            var linqQuery = queryable
                .Skip(req.SkipCount)
                .Take(req.MaxResultCount);

            var shortUrlsTask =
                CosmosSqlHelper<ShortUrlClick>.ToListAsync(_containerClient, linqQuery, _logger,
                    _debugMode);
            var shortUrlsCount =
                queryable.CountAsync(ct);

            return new GetShortUrlClicksResponse()
            {
                TotalCount = await shortUrlsCount,
                Items = (await shortUrlsTask).Select(c => new ShortUrlClickResponse()
                {
                    Id = c.Id,
                    ShortUrlId = c.ShortUrlId,
                    CreationDateTime = c.CreationDateTime,
                    IpAddress = c.IpAddress,
                    IpInfo = $"{c.IpInfo?.City},{c.IpInfo?.Region},{c.IpInfo?.Country} ({c.IpInfo?.Org})",
                    ClientInfo = $"{c.ClientInfo?.OS?.Family} {c.ClientInfo?.OS?.Major} , {c.ClientInfo?.Device?.Brand} {c.ClientInfo?.Device?.Family},{c.ClientInfo?.UA?.Family} {c.ClientInfo?.UA?.Major}"
                }).ToList()
            };
        }
    }
}
