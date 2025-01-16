using Newtonsoft.Json;

namespace LoreVault.Domain.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public required string Id { get; set; }

        [JsonProperty(PropertyName = "GoogleId")]
        public required string GoogleId { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
