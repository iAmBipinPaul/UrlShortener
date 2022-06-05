using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UrlShortenerDbContext>(
        opts => { opts.UseSqlite("UrlShortenerDbContext.db"); });
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(); //add this
var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseFastEndpoints();
app.UseOpenApi(); //add this
app.UseSwaggerUi3(s => s.ConfigureDefaults()); //add this

app.Run();
