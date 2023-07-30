using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UAParser;
using UrlShortener.Applications.Implementations;
using UrlShortener.Applications.Interfaces;
using UrlShortener.Core;
using UrlShortener.Persistence;
using UrlShortener.Shared.Interfaces;
using UrlShortener.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
var connectionString
    = builder.Configuration["ConnectionStrings:DbContext"];
builder.Services.AddDbContext<UrlShortenerContext>(
        opts => { opts.UseNpgsql(connectionString); });
builder.Services.AddCors();
builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
builder.Services.AddScoped<IShortUrlClickService, ShortUrlClickService>();

builder.Services.AddHttpClient<IIpInfoClient, IpInfoClient>();

builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["Auth0_Authority"];
        o.Audience = builder.Configuration["Auth0_Audience"];
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseForwardedHeaders();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/short-url-clicks/{shortUrlId}",
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IShortUrlClickService shortUrlClickService, string shortUrlId,  GetShortUrlClicksRequest request,
            CancellationToken ct) =>
        {
            request.ShortUrlId = shortUrlId;
            var shortUrls = await shortUrlClickService.GetShortUrlClicks(request, ct);
            return Results.Ok(shortUrls);
        }).WithName("GetShortUrlClicks")
    .WithOpenApi();


app.MapPost("api/short-url",
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IShortUrlService shortUrlService, CreateOrUpdateShortUrlRequest request,
            CancellationToken ct) =>
        {
            //TODO need to improve this
            if (!request.ShortName.Contains("\\") && !request.ShortName.Contains("/") &&
                !request.ShortName.Contains(" ") && !request.ShortName.StartsWith("http"))
            {
                var shortUrl = await shortUrlService.CreateShortUrl(request, ct);
                return Results.Ok(shortUrl);
            }

            return Results.Ok();
        }).WithName("CreateShortUrl")
    .WithOpenApi();

app.MapPatch("/api/short-url",
      //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IShortUrlService shortUrlService, CreateOrUpdateShortUrlRequest request,
            CancellationToken ct) =>
        {
            var shortUrl = await shortUrlService.UpdateShortUrl(request, ct);
            if (shortUrl != null)
            {
                return Results.Ok(shortUrl);
            }

            return Results.Ok();
        }).WithName("UpdateShortUrl")
    .WithOpenApi();


app.MapGet("/api/short-urls",
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IShortUrlService shortUrlService,GetShortUrlsRequest request,
            CancellationToken ct) =>
        {
            var shortUrl = await shortUrlService.GetShortUrls(request, ct);
            return Results.Ok(shortUrl);
        }).WithName("GetShortUrls")
    .WithOpenApi();


app.MapDelete("/api/short-url/{shortName}",
      //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (IShortUrlService shortUrlService, string shortName,
            CancellationToken ct) =>
        {
            await shortUrlService.DeleteShortUrl(new DeleteShortUrlRequest()
            {
                ShortName = shortName
            }, ct);
            return Results.Ok();
        }).WithName("DeleteShortUrl")
    .WithOpenApi();



app.MapGet("/{shortName?}",
        async (IShortUrlService shortUrlService, [FromServices] IHttpContextAccessor httpContextAccessor,
            IShortUrlClickService shortUrlClickService, IIpInfoClient ipInfoClient, IConfiguration configuration,
            ILogger<Program> logger, [FromRoute] string? shortName,
            CancellationToken ct) =>
        {
            var defaultUrlForRedirect = configuration.GetValue<string>("DefaultUrlForRedirect") ??
                                        throw new InvalidOperationException();
            IPAddress? ipAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;
            var uaString = httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);

            string redirectUrl = defaultUrlForRedirect;
            if (!string.IsNullOrWhiteSpace(shortName))
            {
                var shortUrlTask = shortUrlService.GetShortUrl(shortName, ct);
                Task<IpInfo?>? getIpInfoTask = null;
                if (ipAddress != null)
                {
                    getIpInfoTask = ipInfoClient.Get(ipAddress.ToString());
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
                            logger.LogError(e, "Failed to get Ip info");
                        }
                    }

                    await shortUrlClickService.InsertShortUrlClick(new InsertShortUrlClickInput()
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
                redirectUrl = defaultUrlForRedirect;
            }

            return Results.Redirect(redirectUrl, permanent: true);
        }).WithName("RedirectToDestination")
    .WithOpenApi();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger/ui"; // <-- Define custom UI route here
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.Run();