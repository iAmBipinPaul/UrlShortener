using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using UrlShortener.Applications.Implementations;
using UrlShortener.Applications.Interfaces;
using UrlShortener.Persistence.Implementations;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;

bool runOnGoogleCloudRun = false;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions> (options => 
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddCors();
builder.Services.AddSingleton<ICosmosDbClient, CosmosDbClient>();
builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
builder.Services.AddScoped<IShortUrlClickService, ShortUrlClickService>();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

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

var app = builder.Build();
app.UseForwardedHeaders();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());

if (runOnGoogleCloudRun)
{
    string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    string url = String.Concat("http://0.0.0.0:", port);
    app.Run(url);
}
else
{
    app.Run();
}
