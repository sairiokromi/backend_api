namespace Kromi.Application.Data.Dto.Auth
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
