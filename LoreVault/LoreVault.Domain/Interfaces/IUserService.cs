using LoreVault.Domain.Models;
using System.Security.Claims;


namespace LoreVault.Domain.Interfaces
{
    public interface IUserService
    {
        Task CreateUserWithRequest(Domain.Models.CreateUserRequest userRequest);
        Task<User> CreateUserWithGoogleId(CreateUserRequest userRequest, string googleId);
        Task<Domain.Models.User> GetUserById(string id);
        Task<IEnumerable<Domain.Models.User>> GetUsers();
        Task<Domain.Models.User> GetUserByGoogleId(string googleId);
        Task<ClaimsPrincipal> VerifyGoogleTokenAsync(string idToken);
    }
}
