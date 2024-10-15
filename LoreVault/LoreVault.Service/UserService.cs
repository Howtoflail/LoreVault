using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;

namespace LoreVault.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateUser(User user)
        {
            await _repository.CreateUser(user);
        }

        public async Task<User> GetUserById(string id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _repository.GetUsers();
        }
    }
}
