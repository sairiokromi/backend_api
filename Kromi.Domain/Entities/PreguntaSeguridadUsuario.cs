using Kromi.Domain.Entities.Contract;

namespace Kromi.Domain.Entities
{
    public class PreguntaSeguridadUsuario : ITimeStamp
    {
        public long Id { get; set; }
        public string? UsuarioId { get; set; } = null!;
        public KromiUser KromiUser { get; set; } = null!;
        public long? PreguntaSeguridadId { get; set; }
        public PreguntaSeguridad PreguntaSeguridad { get; set; } = null!;
        public bool EstaActivo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
