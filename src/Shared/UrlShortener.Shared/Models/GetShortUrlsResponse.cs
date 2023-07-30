namespace UrlShortener.Shared.Models
{
    public class GetShortUrlsResponse
    {

        public long TotalCount { get; set; }
        public List<ShortUrlsResponse> Items { get; set; } = new List<ShortUrlsResponse>();
    }

    public class ShortUrlsResponse
    {
        public string Id { get; set; } = String.Empty;
        public string ShortName { get; set; } = String.Empty;
        public string DestinationUrl { get; set; } = String.Empty;
        public long CreationDateTime { get; set; }
        public long LastUpdateDateTime { get; set; }
    }

}