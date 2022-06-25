using UrlShortener.Shared.Models;

namespace UrlShortener.Shared.Interfaces;

public interface IShortUrlClickService
{
    Task InsertShortUrlClick(InsertShortUrlClickInput input, CancellationToken ct);
    Task<GetShortUrlClicksResponse> GetShortUrlClicks(GetShortUrlClicksRequest req, CancellationToken ct);
}   
 
