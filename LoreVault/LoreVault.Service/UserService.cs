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

        public async Task CreateUserWithRequest(CreateUserRequest userRequest)
        {
            string id = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = id,
                PartitionKey = id,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName
            };

            await _repository.CreateUser(user);
        }

        public async Task CreateUserWithGoogleId(CreateUserRequest userRequest, string googleId)
        {
            var user = new User
            { 
                Id = Guid.NewGuid().ToString(),
                PartitionKey = googleId,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName
            };

            await _repository.CreateUser(user);
        }

        public async Task<User> GetUserByGoogleId(string googleId)
        {
            return await _repository.GetUserByGoogleId(googleId);
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
