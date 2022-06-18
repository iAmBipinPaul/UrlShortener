using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UAParser;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

namespace UrlShortener.Server.EndPoints
{
    public class CreateShortUrlEndpoint : Endpoint<CreateShortUrlRequest, CreateShortUrlResponse>
    {
       
        private IShortUrlService ShortUrlService { get; set; }
        public CreateShortUrlEndpoint(IShortUrlService shortUrlService
        )
        {
            this.ShortUrlService = shortUrlService;
        }


        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/short-url");
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
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
            AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        }

        public override async Task HandleAsync(GetShortUrlsRequest req, CancellationToken ct)
        {
            var shortUrls = await ShortUrlService.GetShortUrls(req, ct);
            await SendAsync(shortUrls, cancellation: ct);
        }
    }

    public class RedirectToDestinationShortUrlsEndpoint : Endpoint<RedirectToDestinationRequest>
    {
        private readonly string _defaultUrlForRedirect;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShortUrlClickService _shortUrlClickService;
        private IShortUrlService ShortUrlService { get; set; }

        public RedirectToDestinationShortUrlsEndpoint(IShortUrlService shortUrlService,

            IHttpContextAccessor httpContextAccessor,
            IShortUrlClickService shortUrlClickService,
            IConfiguration configuration)
        {
            _shortUrlClickService = shortUrlClickService;
            _httpContextAccessor = httpContextAccessor;
            _defaultUrlForRedirect = configuration.GetValue<string>("DefaultUrlForRedirect") ?? throw new InvalidOperationException();
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
            IPAddress? ipAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;
            var uaString = _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString();

            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);

            string redirectUrl = _defaultUrlForRedirect;
            if (!string.IsNullOrWhiteSpace(reqShortUrl.ShortName))
            {
                var shortUrl = await ShortUrlService.GetShortUrl(reqShortUrl.ShortName, ct);

                if (shortUrl != null)
                {
                    redirectUrl = shortUrl.DestinationUrl;
                    await _shortUrlClickService.InsertShortUrlClick(new InsertShortUrlClickInput()
                    {

                        ShortUrlId = shortUrl.ShortName,
                        ClientInfo = c,
                        IpAddress = ipAddress?.ToString(),

                    }, ct);
                }
            }

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = _defaultUrlForRedirect;
            }
            await SendRedirectAsync(redirectUrl, isPermanant: true, cancellation: ct);
        }
    }
}
