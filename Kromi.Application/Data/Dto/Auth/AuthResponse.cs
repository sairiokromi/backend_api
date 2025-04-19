namespace Kromi.Application.Data.Dto.Auth
{
    public class AuthResponse
    {
        public bool EstaActivo { get; set; } = true;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? Firma { get; set; } = null!;
        public string? Ficha { get; set; } = null!;
        public long? SucursalId { get; set; }
        public string? Foto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public IList<string>? Roles { get; set; }
    }
}
