using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreVault.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(Domain.Models.User user);
        Task<Domain.Models.User> GetUserById(string id);
        Task<IEnumerable<Domain.Models.User>> GetUsers();

    }
}
