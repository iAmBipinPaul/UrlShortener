// using FastEndpoints;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using UrlShortener.Shared.Interfaces;
// using UrlShortener.Shared.Models;
//
// namespace UrlShortener.Server.EndPoints
// {
//     public class ShortUrlClickEndPointsEndpoint : Endpoint<GetShortUrlClicksRequest, GetShortUrlClicksResponse>
//     {
//         private IShortUrlClickService ShortUrlClickService { get; set; }
//
//         public ShortUrlClickEndPointsEndpoint(IShortUrlClickService shortUrlClickService)
//         {
//             this.ShortUrlClickService = shortUrlClickService;
//         }
//         public override void Configure()
//         {
//             Verbs(Http.GET);
//             Routes("/api/short-url-clicks/{ShortUrlId}");
//             AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
//         }
//
//         public override async Task HandleAsync(GetShortUrlClicksRequest req, CancellationToken ct)
//         {
//             var shortUrls = await ShortUrlClickService.GetShortUrlClicks(req, ct);
//             await SendAsync(shortUrls, cancellation: ct);
//         }
//     }
//     
// }
