using LinqKit;
using UrlShortener.Domain;
using UrlShortener.Persistence;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UAParser;
using UrlShortener.Core;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlClickService : IShortUrlClickService
    {
        private readonly UrlShortenerContext _urlShortenerContext; 
        public ShortUrlClickService(
            UrlShortenerContext urlShortenerContext)
        {
            _urlShortenerContext = urlShortenerContext;
        }
        public async Task InsertShortUrlClick(InsertShortUrlClickInput input, CancellationToken ct)
        {
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var partitionValue = input.ShortUrlId.FirstOrDefault().ToString().ToLower();
            var shortUrlClick = new ShortUrlClick()
            {
                ShortUrlId = input.ShortUrlId.ToLower(),
                CreationDateTime = unixTimeMilliseconds,
                IpAddress = input.IpAddress,
                ClientInfo = JsonConvert.SerializeObject(input.ClientInfo),
                PartitionValue = partitionValue,
                IpInfo = JsonConvert.SerializeObject(input.IpInfo) ,
                Id = Guid.NewGuid().ToString()
            };
            _urlShortenerContext.ShortUrlClicks.Add(shortUrlClick);
            await _urlShortenerContext.SaveChangesAsync(ct);
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
                Items = res.Select(c =>
                {
                    var clientInfo=  c.ClientInfo is null ?null:JsonConvert.DeserializeObject<ClientInfo>(c.ClientInfo);
                    var ipInfo=  c.IpInfo is null ?null:JsonConvert.DeserializeObject<IpInfo>(c.IpInfo);
                    return new ShortUrlClickResponse()
                    {
                        Id = c.Id,
                        ShortUrlId = c.ShortUrlId,
                        CreationDateTime = c.CreationDateTime,
                        IpAddress = c.IpAddress,
                        IpInfo = $"{ipInfo?.City},{ipInfo?.Region},{ipInfo?.Country} ({ipInfo?.Org})",
                        ClientInfo =
                            $"{clientInfo?.OS?.Family} {clientInfo?.OS?.Major} , {clientInfo?.Device?.Brand} {clientInfo?.Device?.Family},{clientInfo?.UA?.Family} {clientInfo?.UA?.Major}"
                    };
                }).ToList()
            };
        }
    }
}
