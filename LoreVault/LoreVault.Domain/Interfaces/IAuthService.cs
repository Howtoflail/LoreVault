namespace LoreVault.Domain.Interfaces
{
    public interface IAuthService
    {
        public Task<string> GetAccessTokenAsync();
    }
}
