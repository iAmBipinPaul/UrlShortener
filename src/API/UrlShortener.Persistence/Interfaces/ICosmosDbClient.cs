using Microsoft.Azure.Cosmos;

namespace UrlShortener.Persistence.Interfaces
{
    public interface ICosmosDbClient
    {
        Container GetContainerClient();
    } 
}
 