using UAParser;

namespace UrlShortener.Shared.Models
{
    public class InsertShortUrlClickInput
    {
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public string? IpAddress { get; set; }
        public ClientInfo? ClientInfo { get; set; }
    }
}
