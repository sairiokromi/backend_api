using Kromi.Domain.Entities.Contract;

namespace Kromi.Domain.Entities
{
    public class Sucursal : ITimeStamp
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public bool EstaActivo { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public ICollection<KromiUser> Usuarios { get; set; } = null!;
    }
}
