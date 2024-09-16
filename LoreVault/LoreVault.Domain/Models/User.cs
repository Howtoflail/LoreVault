using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreVault.Domain.Models
{
    [DynamoDBTable("users")]
    public class User
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("first_name")]
        public string FirstName { get; set; }

        [DynamoDBProperty("last_name")]
        public string LastName { get; set; }
    }
}
