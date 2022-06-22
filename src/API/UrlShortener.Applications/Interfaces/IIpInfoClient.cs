using UrlShortener.Core;

namespace UrlShortener.Applications.Interfaces
{
    public interface IIpInfoClient
    {
        Task<IpInfo?> Get(string ipAddress);
    }
}
