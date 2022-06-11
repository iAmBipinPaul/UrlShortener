using FastEndpoints;
using FastEndpoints.Swagger;
using UrlShortener.Applications;
using UrlShortener.Persistence.Implementations;
using UrlShortener.Persistence.Interfaces;
using UrlShortener.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICosmosDbClient, CosmosDbClient>();
builder.Services.AddScoped<IShortUrlService,ShortUrlService>();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(); //add this
var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseFastEndpoints();
app.UseOpenApi(); //add this
app.UseSwaggerUi3(s => s.ConfigureDefaults()); //add this

app.Run();
