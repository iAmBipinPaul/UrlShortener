using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UrlShortener.Applications;
using UrlShortener.Persistence.Implementations;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddSingleton<ICosmosDbClient, CosmosDbClient>();
builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();



builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["Auth0:Authority"];
        o.Audience = builder.Configuration["Auth0:Audience"];
    });

var app = builder.Build();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());


app.Run();
