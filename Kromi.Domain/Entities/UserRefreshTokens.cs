using Kromi.Domain.Entities.Contract;

namespace Kromi.Domain.Entities
{
    public class UserRefreshTokens : ITimeStamp
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
