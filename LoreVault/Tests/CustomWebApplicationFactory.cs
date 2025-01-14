using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using LoreVault.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Add appsettings.json to the configuration
                configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices((context, services) =>
            {
                // Get CosmosDbTest settings from the configuration
                var cosmosDbTestSettings = context.Configuration.GetSection("CosmosDbTest");

                // Remove the existing CosmosDb configuration
                var cosmosDbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IOptions<CosmosDbSettings>));
                if (cosmosDbDescriptor != null)
                {
                    services.Remove(cosmosDbDescriptor);
                }

                // Bind CosmosDbTest settings
                services.Configure<CosmosDbSettings>(cosmosDbTestSettings);

                // Seed the test database
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var cosmosDbSettings = scope.ServiceProvider.GetRequiredService<IOptions<CosmosDbSettings>>().Value;
                InitializeTestData(cosmosDbSettings).GetAwaiter().GetResult();
            });
        }

        private async Task InitializeTestData(CosmosDbSettings cosmosDbSettings)
        {
            var client = new CosmosClient(cosmosDbSettings.EndpointUri, cosmosDbSettings.PrimaryKey);
            var database = await client.CreateDatabaseIfNotExistsAsync(cosmosDbSettings.DatabaseId);
            var container = await database.Database.CreateContainerIfNotExistsAsync(cosmosDbSettings.ContainerId, "/GoogleId");

            // Seed test data
            var testUsers = new List<dynamic>
            {
                new { id = "1", GoogleId = "google-oauth2|109027151390841274362", FirstName = "John", LastName = "Doe" },
                new { id = "2", GoogleId = "google-oauth2|109027151390841275253", FirstName = "Jane", LastName = "Smith" }
            };

            foreach (var testUser in testUsers)
            {
                await container.Container.UpsertItemAsync(testUser, new PartitionKey(testUser.GoogleId));
            }
        }
    }
}
