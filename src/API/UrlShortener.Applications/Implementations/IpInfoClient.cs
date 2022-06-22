using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using UrlShortener.Applications.Interfaces;
using UrlShortener.Core;

namespace UrlShortener.Applications.Implementations
{
    public class IpInfoClient : IIpInfoClient
    {
        private readonly string? _ipInfoToken;
        private  readonly HttpClient _httpClient;

        public IpInfoClient(IConfiguration configuration,HttpClient httpClient)
        {
            _httpClient = httpClient;
            httpClient.BaseAddress = new Uri("https://ipinfo.io");
            _ipInfoToken = configuration.GetValue<string>("IpInfoToken");
        }

        public async Task<IpInfo?> Get(string ipAddress)
        {
            if (!string.IsNullOrWhiteSpace(_ipInfoToken))
            {
                var res = await _httpClient.GetFromJsonAsync<IpInfo>($"/{ipAddress}?token={_ipInfoToken}");
                return res;
            }

            return null;
        }
    }
}
