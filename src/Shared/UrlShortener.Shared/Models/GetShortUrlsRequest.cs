using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace UrlShortener.Shared.Models
{
    public class GetShortUrlsRequest
    {
        public string? Query { get; set; }
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 10;


        public static ValueTask<GetShortUrlsRequest?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            string author = context.Request.Query["Query"];
            int.TryParse(context.Request.Query["SkipCount"], out var skipCount);
            int.TryParse(context.Request.Query["MaxResultCount"], out var maxResultCount);
            var result = new GetShortUrlsRequest
            {
                Query = author,
                SkipCount = skipCount,
                MaxResultCount= maxResultCount
            };
            if (result.MaxResultCount==0)
            {
                result.MaxResultCount = 10;
            }
            return ValueTask.FromResult<GetShortUrlsRequest?>(result);
        }
    }
}
 