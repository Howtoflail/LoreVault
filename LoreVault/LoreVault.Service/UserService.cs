using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LoreVault.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private Auth0Settings _settings;

        public UserService(IUserRepository repository, IOptions<Auth0Settings> auth0Settings)
        {
            _repository = repository;
            _settings = auth0Settings.Value;
        }

        public async Task CreateUserWithRequest(CreateUserRequest userRequest)
        {
            string id = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = id,
                GoogleId = id,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName
            };

            await _repository.CreateUser(user);
        }

        public async Task<User> CreateUserWithGoogleId(CreateUserRequest userRequest, string googleId)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                GoogleId = googleId,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName
            };

            return (User)await _repository.CreateUser(user);
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

        public async Task<ClaimsPrincipal> VerifyGoogleTokenAsync(string idToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://{_settings.Domain}/", // Auth0 issuer from the token's "iss" field 

                ValidateAudience = true,
                ValidAudience = _settings.ClientId, // Auth0 audience from the token's "aud" field

                ValidateLifetime = true, // Ensure the token is not expired
                ClockSkew = TimeSpan.FromMinutes(5), // Allow for some clock skew

                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // Fetch Auth0's public keys dynamically
                    var client = new HttpClient();
                    var json = client.GetStringAsync($"https://{_settings.Domain}/.well-known/jwks.json").Result;
                    var keys = new JsonWebKeySet(json);
                    return keys.Keys;
                }
            };

            var handler = new JwtSecurityTokenHandler();

            try
            {
                // Validate token and return claims
                var principal = handler.ValidateToken(idToken, validationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                // Token validation failed
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
