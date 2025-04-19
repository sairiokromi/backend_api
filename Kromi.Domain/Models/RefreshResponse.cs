namespace Kromi.Domain.Models
{
    public class RefreshResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
