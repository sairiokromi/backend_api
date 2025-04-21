using Kromi.Domain.Entities.Contract;
using Microsoft.AspNetCore.Identity;

namespace Kromi.Domain.Entities
{
    public class KromiUser : IdentityUser, IAuditable
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool EstaActivo { get; set; } = true;
        public string Nombres { get; set; } = null!;
        public string? Apellidos { get; set; }
        public string? Foto { get; set; }
        public string? Firma { get; set; }
        public string? Ficha { get; set; }
        public DateTime? BloqueadoEl { get; set; }
        public long? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        public ICollection<PreguntaSeguridadUsuario> PreguntaSeguridadUsuario { get; set; } = [];
    }
}
