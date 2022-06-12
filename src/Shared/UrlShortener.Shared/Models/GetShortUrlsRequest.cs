namespace UrlShortener.Shared.Models
{
    public class GetShortUrlsRequest
    {
        public string? Query { get; set; }
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 10;
    }
}
