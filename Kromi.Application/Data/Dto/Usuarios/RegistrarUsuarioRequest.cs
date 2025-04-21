namespace Kromi.Application.Data.Dto.Usuarios
{
    public class RegistrarUsuarioRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Firma { get; set; }
        public string? Ficha { get; set; }
        public int? SucursalId { get; set; }
        public string? Foto { get; set; }
        public string[] Role { get; set; } = null!;
    }
}
