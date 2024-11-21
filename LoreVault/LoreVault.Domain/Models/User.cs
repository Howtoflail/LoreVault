using Newtonsoft.Json;
using System;

namespace LoreVault.Domain.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "GoogleId")]
        public string GoogleId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
