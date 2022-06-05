using FastEndpoints;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Server.EndPoints
{
    public class CreateShortUrlEndpoint : Endpoint<CreateShortUrlRequest, CreateShortUrlResponse>
    {
        private IShortUrlService ShortUrlService { get; set; }

        public CreateShortUrlEndpoint(IShortUrlService shortUrlService)
        {
            this.ShortUrlService = shortUrlService;
        }


        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/short-url");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateShortUrlRequest req, CancellationToken ct)
        {
            var shortUrl = await ShortUrlService.CreateShortUrl(req, ct);
            await SendAsync(shortUrl, cancellation: ct);
        }
    }

    public class GetShortUrlsEndpoint : Endpoint<GetShortUrlsRequest, GetShortUrlsResponse>
    {
        private IShortUrlService ShortUrlService { get; set; }

        public GetShortUrlsEndpoint(IShortUrlService shortUrlService)
        {
            this.ShortUrlService = shortUrlService;
        }


        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("/api/short-urls");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetShortUrlsRequest req, CancellationToken ct)
        {
            var shortUrls = await ShortUrlService.GetShortUrls(req, ct);
            await SendAsync(shortUrls, cancellation: ct);
        }
    }

    public class RedirectToDestinationShortUrlsEndpoint : Endpoint<RedirectToDestinationRequest>
    {
        private IShortUrlService ShortUrlService { get; set; }

        public RedirectToDestinationShortUrlsEndpoint(IShortUrlService shortUrlService)
        {
            this.ShortUrlService = shortUrlService;
        }


        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("/{ShortName?}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(RedirectToDestinationRequest reqShortUrl, CancellationToken ct)
        {
            var redirectUrl = "https://bipinpaul.com";
            if (!string.IsNullOrWhiteSpace(reqShortUrl.ShortName))
            {
                var shortUrl = await ShortUrlService.GetShortUrl(reqShortUrl.ShortName, ct);
                if (shortUrl != null)
                {
                    redirectUrl = shortUrl.DestinationUrl;
                }
            }
            await SendRedirectAsync(redirectUrl, isPermanant: true, cancellation: ct);
        }
    }
}
