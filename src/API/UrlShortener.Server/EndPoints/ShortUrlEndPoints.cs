using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UAParser;
using UrlShortener.Applications.Interfaces;
using UrlShortener.Core;
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
        private readonly IIpInfoClient _ipInfoClient;
        private readonly ILogger<RedirectToDestinationShortUrlsEndpoint> _logger;
        private readonly IShortUrlService _shortUrlService;
      

        public RedirectToDestinationShortUrlsEndpoint(IShortUrlService shortUrlService,
            IHttpContextAccessor httpContextAccessor,
            IShortUrlClickService shortUrlClickService,
            IIpInfoClient ipInfoClient,
            IConfiguration configuration,
            ILogger<RedirectToDestinationShortUrlsEndpoint> logger
            
            )
        {
            _logger = logger;
            _ipInfoClient = ipInfoClient;
            _shortUrlClickService = shortUrlClickService;
            _httpContextAccessor = httpContextAccessor;
            _defaultUrlForRedirect = configuration.GetValue<string>("DefaultUrlForRedirect") ??
                                     throw new InvalidOperationException();
            this._shortUrlService = shortUrlService;
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
                var shortUrlTask = _shortUrlService.GetShortUrl(reqShortUrl.ShortName, ct);
                Task<IpInfo?>? getIpInfoTask = null;
                if (ipAddress != null)
                {
                    getIpInfoTask = _ipInfoClient.Get(ipAddress.ToString());
                }

                var shortUrl = await shortUrlTask;

                if (shortUrl != null) 
                {
                    IpInfo? ipInfo = null;
                    redirectUrl = shortUrl.DestinationUrl;
                    if (getIpInfoTask != null)
                    {
                        try
                        {
                            ipInfo = await getIpInfoTask;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to get Ip info");
                        }
                    }
                    await _shortUrlClickService.InsertShortUrlClick(new InsertShortUrlClickInput()
                    {
                        ShortUrlId = shortUrl.ShortName,
                        ClientInfo = c,
                        IpAddress = ipAddress?.ToString(),
                        IpInfo = ipInfo
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
