using UrlShortener.Shared.Models;

namespace UrlShortener.Shared.Interfaces
{
    public interface IShortUrlService
    {
        Task<GetShortUrlsResponse> GetShortUrls(GetShortUrlsRequest req, CancellationToken ct);
        Task<CreateOrUpdateShortUrlResponse> CreateShortUrl(CreateOrUpdateShortUrlRequest req, CancellationToken ct);
        Task<ShortUrlsResponse?> GetShortUrl(string shortName, CancellationToken ct);
        Task DeleteShortUrl(DeleteShortUrlRequest req, CancellationToken ct);
        Task<CreateOrUpdateShortUrlResponse?> UpdateShortUrl(CreateOrUpdateShortUrlRequest req, CancellationToken ct);
    }
} 
