using Newtonsoft.Json;

namespace UrlShortener.Domain
{
    public class ShortUrlClick
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public EntityKind EntityKind { get; set; } = EntityKind.ShortUrl;

        public string PartitionValue { get; set; }
    }
}
