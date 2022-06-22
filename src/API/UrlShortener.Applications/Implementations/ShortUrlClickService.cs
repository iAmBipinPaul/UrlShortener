using Microsoft.Azure.Cosmos;
using UrlShortener.Domain;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Applications.Implementations
{
    public class ShortUrlClickService : IShortUrlClickService
    {
        private readonly Container _containerClient;
        public ShortUrlClickService(ICosmosDbClient cosmosDbClient)
        {
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
    }
}
