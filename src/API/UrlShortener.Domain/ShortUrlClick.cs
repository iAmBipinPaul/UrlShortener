using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using UAParser;
using UrlShortener.Core;

namespace UrlShortener.Domain
{
    public class ShortUrlClick
    {
        [JsonProperty("id")]
        public string Id { get; set; } = String.Empty;
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public EntityKind EntityKind { get; set; } = EntityKind.ShortUrlClick;
        public string PartitionValue { get; set; }
        public string? IpAddress { get; set; }
        [Column(TypeName = "jsonb")]
        public IpInfo? IpInfo { get; set; }
        [Column(TypeName = "jsonb")]
        public string? ClientInfo { get; set; }
        public int? ttl { get; set; } = 7889238; //3 month
        [JsonIgnore]
        public ShortUrl ShortUrl { get; set; }
    }
    
} 
