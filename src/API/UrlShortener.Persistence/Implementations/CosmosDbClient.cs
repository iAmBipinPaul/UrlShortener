using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using UrlShortener.Persistence.Interfaces;

namespace UrlShortener.Persistence.Implementations
{
    public class CosmosDbClient : ICosmosDbClient
    {

        private readonly Container _container;

        public CosmosDbClient(IConfiguration configuration)
        {
            var cosmosDbConnectionString = configuration.GetValue<string>("CosmosDbConnectionString");
            var database = configuration.GetValue<string>("CosmosDbDatabase");
            var container = configuration.GetValue<string>("Container");
            if (cosmosDbConnectionString is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var client = new CosmosClient(cosmosDbConnectionString, new CosmosClientOptions
            {
                AllowBulkExecution = true
            });

            _container = client.GetDatabase(database)
               .GetContainer(container);
        }

        public Container GetContainerClient()
        {
            return _container;
        }
    }
}
