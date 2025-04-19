using Kromi.Domain.Entities;
using System.Security.Claims;

namespace Kromi.Infrastructure.Contracts.Identity
{
    public interface IJwtService
    {
        string? GetSessionUser();
        string? GetSessionUserId();
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateUserAccessToken(KromiUser usuario, IList<string>? roles);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
