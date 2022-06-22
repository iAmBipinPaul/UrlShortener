using UAParser;
using UrlShortener.Core;

namespace UrlShortener.Shared.Models
{
    public class InsertShortUrlClickInput
    {
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public string? IpAddress { get; set; }
        public ClientInfo? ClientInfo { get; set; }
        public IpInfo? IpInfo { get; set; }
    }
}
