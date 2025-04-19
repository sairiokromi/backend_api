using Kromi.Domain.Entities;
using Kromi.Infrastructure.Contracts.Identity;
using Kromi.Infrastructure.SettingsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Kromi.Infrastructure.Specifications
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSettings _jwtSettings;

        public JwtService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtSettings = jwtSettings.Value;
        }

        public string? GetSessionUser()
        => _httpContextAccessor.HttpContext!.User?.Claims?
            .FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Name)?.Value;

        public string? GetSessionUserId()
        => (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext!.User?.Claims != null) 
                ? _httpContextAccessor.HttpContext!.User?.Claims?.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.NameId)?.Value 
                : null;

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateUserAccessToken(KromiUser usuario, IList<string>? roles)
        {
            var claims = new List<Claim> {
                new (JwtRegisteredClaimNames.Name, usuario.UserName!),
                new (JwtRegisteredClaimNames.NameId, usuario.Id)
            };
            foreach (var role in roles!)
            {
                var claim = new Claim(ClaimTypes.Role, role);
                claims.Add(claim);
            }
            return GenerateAccessToken(claims);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
