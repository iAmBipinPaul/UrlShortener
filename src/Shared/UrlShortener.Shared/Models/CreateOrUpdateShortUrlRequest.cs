namespace UrlShortener.Shared.Models
{
    public class CreateOrUpdateShortUrlRequest
    {
        public string ShortName { get; set; } = String.Empty;
        public string DestinationUrl { get; set; } = String.Empty;
        public bool IsEdit { get; set; }
    } 
}
