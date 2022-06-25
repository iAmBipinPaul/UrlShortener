namespace UrlShortener.Shared.Models;

public class GetShortUrlClicksResponse
{
    public long TotalCount { get; set; }
    public List<ShortUrlClickResponse> Items { get; set; } = new List<ShortUrlClickResponse>();
}
