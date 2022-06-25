namespace UrlShortener.Shared.Models;

public class ShortUrlClickResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ShortUrlId { get; set; } = String.Empty;
    public long CreationDateTime { get; set; }
    public string? IpAddress { get; set; }
    public string? IpInfo { get; set; }
    public string? ClientInfo { get; set; }
}
