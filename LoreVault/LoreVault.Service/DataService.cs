using RestSharp;
using LoreVault.Domain.Interfaces;

namespace LoreVault.Service
{
    public class DataService : IDataService
    {
        private readonly IAuthService _authService;

        public DataService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RestResponse> CallSecureApiAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var client = new RestClient("http://localhost:3010/api/private");
            var request = new RestRequest(Method.Get.ToString());
            request.AddHeader("authorization", $"Bearer {token}");
            var response = await client.ExecuteAsync(request);

            return response;
        }
    }
}
