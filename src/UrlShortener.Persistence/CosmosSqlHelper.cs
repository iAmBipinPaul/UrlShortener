using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Persistence
{
    public static class CosmosSqlHelper<T>
    {
        public static async Task<List<T>> ToListAsync(Container cosmosDbSqlContainerClient, IQueryable<T> queryable,
            ILogger logger = null, bool debugMode = false)
        {
            var list = new List<T>();
            var iterator =
                cosmosDbSqlContainerClient.GetItemQueryIterator<T>(queryable.ToQueryDefinition());
            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync();

                if (debugMode)
                {
                    logger?.LogInformation($"RU charge: {page.RequestCharge}");
                }

                foreach (var item in page)
                {
                    if (!list.Contains((T)item))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        public static async Task<List<T>> ToListAsync(Container cosmosDbSqlContainerClient, string query,
            ILogger logger = null, bool debugMode = false)
        {
            var list = new List<T>();
            var iterator =
                cosmosDbSqlContainerClient.GetItemQueryIterator<T>(query);
            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync();
                if (debugMode)
                {
                    logger?.LogInformation($"RU charge: {page.RequestCharge}");
                }

                foreach (var item in page)
                {
                    if (!list.Contains((T)item))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
