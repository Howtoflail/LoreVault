namespace LoreVault.Domain.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GetAccessTokenAsync();

        public Task<string> GetUserAccessTokenAsync(string authorizationCode, string redirectUri);
    }
}
