namespace Tests.Integration
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory) 
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnUsers()
        {
            // Arrange
            var userId = "1";

            // Act
            var response = await _client.GetAsync($"/api/User/get-users");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
            Assert.Contains(userId, responseContent);
        }
    }
}
