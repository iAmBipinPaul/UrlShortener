using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace UrlShortener.Shared.Models;

public class GetShortUrlClicksRequest
{
    public string? Query { get; set; }
    public string ShortUrlId { get; set; } = string.Empty;
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 10;

    public static ValueTask<GetShortUrlClicksRequest?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        string author = context.Request.Query["Query"];
        string shortUrlId = context.Request.Query["ShortUrlId"];
        int.TryParse(context.Request.Query["SkipCount"], out var skipCount);
        int.TryParse(context.Request.Query["MaxResultCount"], out var maxResultCount);
        var result = new GetShortUrlClicksRequest
        {
            Query = author,
            ShortUrlId = shortUrlId,
            SkipCount = skipCount,
            MaxResultCount = maxResultCount
        };
        return ValueTask.FromResult<GetShortUrlClicksRequest?>(result);
    }

}
