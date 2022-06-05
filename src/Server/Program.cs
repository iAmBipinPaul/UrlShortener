using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Applications;
using UrlShortener.Persistence;
using UrlShortener.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UrlShortenerDbContext>(
        opts => { opts.UseSqlite("Data Source=UrlShortener.db"); });
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
