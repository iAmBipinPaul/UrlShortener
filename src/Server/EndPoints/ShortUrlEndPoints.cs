using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence;
using UrlShortener.Shared.Models;

namespace UrlShortener.Server.EndPoints
{
    public class CreateShortUrlEndpoint : Endpoint<CreateShortUrlRequest, CreateShortUrlResponse>
    {
        public UrlShortenerDbContext DbContext { get; set; }

        public override void Configure()
        {
            Verbs(Http.POST);
            Routes("/api/short-url");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateShortUrlRequest req, CancellationToken ct)
        {
            var response = new CreateShortUrlResponse()
            {
                //FullName = req.FirstName + " " + req.LastName,
                //IsOver18 = req.Age > 18
            };
            await SendAsync(response, cancellation: ct);
        }
    }

    public class GetShortUrlsEndpoint : Endpoint<GetShortUrlsRequest, GetShortUrlsResponse>
    {
        public UrlShortenerDbContext DbContext { get; set; }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("/api/short-urls");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetShortUrlsRequest req, CancellationToken ct)
        {
            var listAsync =await DbContext.ShortUrls.ToListAsync(cancellationToken: ct);
            // var response = new GetShortUrlsResponse()
            // {
            //     //FullName = req.FirstName + " " + req.LastName,
            //     //IsOver18 = req.Age > 18
            // };
            await SendAsync(new GetShortUrlsResponse(), cancellation: ct);
        }
    }

    public class RedirectToDestinationShortUrlsEndpoint : Endpoint<RedirectToDestinationRequest>
    {
        public UrlShortenerDbContext DbContext { get; set; }

        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("/{ShortUrl}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(RedirectToDestinationRequest reqShortUrl, CancellationToken ct)
        {
            await SendRedirectAsync("https://google.com", isPermanant: true, cancellation: ct);
        }
    }
}
