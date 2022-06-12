using Refit;
using UrlShortener.Shared.Models;

namespace UrlShortener.UI.Blazor.Interfaces
{
    public interface IRefitShortUrlService
    {
        [Get("/api/short-urls")]

        Task<GetShortUrlsResponse> GetShortUrls(GetShortUrlsRequest req);
        [Post("/api/short-url")]
        Task<CreateShortUrlResponse> CreateShortUrl(CreateShortUrlRequest req);
    }
}
//IAccessTokenProvider