using Newtonsoft.Json;
using UAParser;

namespace UrlShortener.Domain
{
    public class ShortUrlClick
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public EntityKind EntityKind { get; set; } = EntityKind.ShortUrlClick;
        public string PartitionValue { get; set; }
        public string? IpAddress { get; set; }
        public ClientInfo? ClientInfo { get; set; }
        public int? ttl { get; set; } = 7889238; //3 month
    }
} 
