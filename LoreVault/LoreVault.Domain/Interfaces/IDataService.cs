using RestSharp;

namespace LoreVault.Domain.Interfaces
{
    public interface IDataService
    {
        public Task<RestResponse> CallSecureApiAsync();
    }
}
