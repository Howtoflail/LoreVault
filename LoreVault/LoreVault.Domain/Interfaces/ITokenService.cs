using LoreVault.Domain.Models;

namespace LoreVault.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwt(User user, string idToken);
    }
}
