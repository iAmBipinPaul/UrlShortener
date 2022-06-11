namespace UrlShortener.Shared.Models
{
    public class CreateShortUrlRequest
    {
        public string ShortName { get; set; } = String.Empty;
        public string DestinationUrl { get; set; } = String.Empty;
        public string TempPassKey { get; set; } = String.Empty;
    }
}
 