namespace UrlShortener.Shared.Models
{
    public class GetShortUrlsRequest
    {
        public string? Query { get; set; }
        public long SkipCount { get; set; }
        public long MazResultCount { get; set; }
    }
}
  