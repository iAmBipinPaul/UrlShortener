using UrlShortener.Shared.Models;

namespace UrlShortener.Shared.Interfaces
{
    public interface IShortUrlService
    {
        Task<GetShortUrlsResponse> GetShortUrls(GetShortUrlsRequest req, CancellationToken ct);
        Task<CreateShortUrlResponse> CreateShortUrl(CreateShortUrlRequest req, CancellationToken ct);
        Task<ShortUrlsResponse?> GetShortUrl(string shortName, CancellationToken ct);
    }
}
