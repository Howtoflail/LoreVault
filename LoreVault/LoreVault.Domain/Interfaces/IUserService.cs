using LoreVault.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreVault.Domain.Interfaces
{
    public interface IUserService
    {
        Task CreateUserWithRequest(Domain.Models.CreateUserRequest userRequest);
        Task CreateUserWithGoogleId(CreateUserRequest userRequest, string googleId);
        Task<Domain.Models.User> GetUserById(string id);
        Task<IEnumerable<Domain.Models.User>> GetUsers();
        Task<Domain.Models.User> GetUserByGoogleId(string googleId);
    }
}
