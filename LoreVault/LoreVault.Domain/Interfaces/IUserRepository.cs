using Microsoft.Azure.Cosmos;

namespace LoreVault.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ItemResponse<Domain.Models.User>> CreateUser(Domain.Models.User user);
        Task<Domain.Models.User> GetUserById(string id);
        Task<IEnumerable<Domain.Models.User>> GetUsers();
        Task<Domain.Models.User> GetUserByGoogleId(string googleId);

    }
}
