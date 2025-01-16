using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoreVault.Service
{
    public class TokenService : ITokenService
    {
        private Auth0Settings _settings;

        public TokenService(IOptions<Auth0Settings> auth0Settings)
        {
            _settings = auth0Settings.Value;
        }

        public string GenerateJwt(User user, string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(idToken) as JwtSecurityToken;

            if (jwtToken == null) 
            {
                throw new ArgumentException("Invalid frontend token.");
            }

            // Extract the 'iat' and 'exp' claims from the frontend token
            var iat = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value;
            var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (iat == null || exp == null)
            {
                throw new ArgumentException("Frontend token missing iat or exp claims.");
            }

            // Ensure 'iat' and 'exp' are parsed correctly as long
            if (!long.TryParse(iat, out var iatTimestamp) || !long.TryParse(exp, out var expTimestamp))
            {
                throw new ArgumentException("Invalid 'iat' or 'exp' format in frontend token.");
            }

            // Set custom claims for backend JWT
            var claims = new[]
            {
                new Claim("FirstName", user.FirstName),
                new Claim("FamilyName", user.LastName),
                // new Claim(ClaimTypes.Email, user.Email),
                new Claim("GoogleId", user.GoogleId),  // Google ID
                // new Claim("role", user.Role),
                // new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.FromUnixTimeSeconds(iatTimestamp).DateTime.ToString(), ClaimValueTypes.DateTime),
                // new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.FromUnixTimeSeconds(expTimestamp).DateTime.ToString(), ClaimValueTypes.DateTime),
                new Claim(JwtRegisteredClaimNames.Iat, iatTimestamp.ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, expTimestamp.ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token with the same iat and exp from the frontend token
            var token = new JwtSecurityToken(
                issuer: $"https://{_settings.Domain}/",
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTimeOffset.FromUnixTimeSeconds(iatTimestamp).DateTime,
                expires: DateTimeOffset.FromUnixTimeSeconds(expTimestamp).DateTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
