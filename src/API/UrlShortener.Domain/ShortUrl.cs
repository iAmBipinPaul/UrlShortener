
using Newtonsoft.Json;

namespace UrlShortener.Domain
{
    public class ShortUrl
    {
      
        [JsonProperty("id")]
        public string ShortName { get; set; } = String.Empty;
        public string DestinationUrl { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public long LastUpdateDateTime { get; set; }
        public EntityKind EntityKind { get; set; } = EntityKind.ShortUrl;
        public string PartitionValue { get; set; }
        public int? ttl { get; set; } = -1;
    }
} 
