using LinqKit;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlClickService : IShortUrlClickService
    {
        private readonly Container _containerClient;
        private readonly UrlShortenerContext _urlShortenerContext; 
        public ShortUrlClickService(ICosmosDbClient cosmosDbClient,IConfiguration configuration,
            UrlShortenerContext urlShortenerContext,
            ILogger<ShortUrlClickService> logger)
        {
            _urlShortenerContext = urlShortenerContext;
            _containerClient = cosmosDbClient.GetContainerClient();
        }
        public async Task InsertShortUrlClick(InsertShortUrlClickInput input, CancellationToken ct)
        {
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var partitionValue = input.ShortUrlId.FirstOrDefault().ToString().ToLower();
            var shortUrlClick = new ShortUrlClick()
            {
                ShortUrlId = input.ShortUrlId,
                CreationDateTime = unixTimeMilliseconds,
                IpAddress = input.IpAddress,
                ClientInfo = input.ClientInfo,
                PartitionValue = partitionValue,
                IpInfo = input.IpInfo
            };
            var tasks = new List<Task> { _containerClient.CreateItemAsync(shortUrlClick, cancellationToken: ct) };
            _urlShortenerContext.ShortUrlClicks.Add(shortUrlClick);
            tasks.Add(_urlShortenerContext.SaveChangesAsync(ct));
            await Task.WhenAll(tasks);
        }

        public async Task<GetShortUrlClicksResponse> GetShortUrlClicks(GetShortUrlClicksRequest req, CancellationToken ct)
        {
            var predicate = PredicateBuilder.New<ShortUrlClick>(true);
            predicate = predicate.And(p =>
                p.ShortUrlId==req.ShortUrlId 
            );
            var queryable =  _urlShortenerContext.ShortUrlClicks
                .Where(predicate);

            var linqQuery = queryable
                .OrderByDescending(c => c.CreationDateTime)
                .Skip(req.SkipCount)
                .Take(req.MaxResultCount);
            

            var res=await linqQuery.ToListAsync(ct);
            return new GetShortUrlClicksResponse()
            {
                TotalCount =await queryable.CountAsync(ct),
                Items = res.Select(c => new ShortUrlClickResponse()
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
