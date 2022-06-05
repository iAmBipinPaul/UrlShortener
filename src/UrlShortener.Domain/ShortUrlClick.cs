namespace UrlShortener.Domain
{
    public class ShortUrlClick
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ShortUrlId { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public ShortUrl ShortUrl { get; set; }
    }
}
