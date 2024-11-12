using LoreVault.Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace LoreVault.DAL
{
    public class UserRepository : IUserRepository
    {
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;

        // The name of the database and container we will create
        private readonly string databaseId;
        private readonly string containerId;

        public UserRepository(IOptions<CosmosDbSettings> cosmosDbSettings)
        {
            var settings = cosmosDbSettings.Value;

            cosmosClient = new CosmosClient(settings.EndpointUri, settings.PrimaryKey, new CosmosClientOptions() { ApplicationName = "LoreVault" });
            databaseId = settings.DatabaseId;
            containerId = settings.ContainerId;

            InitializeDatabase().Wait();
        }

        private async Task InitializeDatabase()
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
        }

        public async Task CreateUser(Domain.Models.User user)
        {
            await container.CreateItemAsync<Domain.Models.User>(user, new PartitionKey(user.Id.ToString()));
        }

        public async Task<IEnumerable<Domain.Models.User>> GetUsers()
        {
            var query = "SELECT * FROM Users";
            QueryDefinition queryDefinition = new QueryDefinition(query);
            FeedIterator<Domain.Models.User> queryResultSetIterator = container.GetItemQueryIterator<Domain.Models.User>(queryDefinition);

            List<Domain.Models.User> users = new List<Domain.Models.User>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Domain.Models.User> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Domain.Models.User user in currentResultSet)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<Domain.Models.User> GetUserById(string id)
        {
            try
            {
                ItemResponse<Domain.Models.User> response = await container.ReadItemAsync<Domain.Models.User>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                throw;
            }
        }

        // Make function async
        public async Task<Domain.Models.User> GetUserByGoogleId(string googleId)
        {
            try 
            {
                var query = container.GetItemLinqQueryable<Domain.Models.User>(true)
                    .Where(u => u.PartitionKey == googleId)
                    .AsEnumerable()
                    .FirstOrDefault();

                return query;
            }
            catch (CosmosException ex)
            {
                throw;
            }
        }
    }
}
