using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text.Json;
using LoreVault.Domain.Interfaces;

namespace LoreVault.Service
{
    public class AuthService : IAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _audience;
        private readonly string _domain;

        public AuthService(IConfiguration configuration)
        {
            _clientId = configuration["Auth0:ClientId"];
            _clientSecret = configuration["Auth0:ClientSecret"];
            _audience = configuration["Auth0:Audience"];
            _domain = configuration["Auth0:Domain"];
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = new RestClient($"https://{_domain}/oauth/token");
            var request = new RestRequest(Method.Post.ToString());
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", 
                $"grant_type=client_credentials&client_id={_clientId}&client_secret={_clientSecret}&audience={_audience}", 
                ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful) 
            {
                var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(response.Content);
                return tokenResponse?.AccessToken;
            }

            throw new Exception("Could not retrieve access token");
        }

        public async Task<string> GetUserAccessTokenAsync(string authorizationCode, string redirectUri)
        {
            var client = new RestClient($"https://{_domain}/oauth/token");
            var request = new RestRequest(Method.Post.ToString());
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                $"grant_type=authorization_code&client_id={_clientId}&client_secret={_clientSecret}&code={authorizationCode}&redirect_uri={redirectUri}",
                ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(response.Content);
                return tokenResponse?.AccessToken;
            }

            throw new Exception("Could not retrieve user access token");
        }
    }
}
