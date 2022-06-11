namespace UrlShortener.Shared.Models
{
    public class GetShortUrlsRequest
    {
        public string? Query { get; set; }
        public int SkipCount { get; set; }
        public int MazResultCount { get; set; }
    }
}
