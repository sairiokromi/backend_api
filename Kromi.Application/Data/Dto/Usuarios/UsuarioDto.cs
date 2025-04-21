namespace Kromi.Application.Data.Dto.Usuarios
{
    public class UsuarioDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EstaActivo { get; set; } = true;
        public string Nombres { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Firma { get; set; }
        public string? Ficha { get; set; }
        public string? Foto { get; set; }
        public long? SucursalId { get; set; }
        public string? Sucursal { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? BloqueadoEl { get; set; }
        public IList<string> Roles { get; set; } = [];
    }
}
