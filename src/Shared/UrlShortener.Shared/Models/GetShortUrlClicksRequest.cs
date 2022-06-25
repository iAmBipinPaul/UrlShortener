namespace UrlShortener.Shared.Models;

public class GetShortUrlClicksRequest
{
    public string? Query { get; set; }
    public string ShortUrlId { get; set; } = string.Empty;
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 10;
  
}
