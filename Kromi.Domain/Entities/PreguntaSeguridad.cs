using Kromi.Domain.Entities.Contract;

namespace Kromi.Domain.Entities
{
    public class PreguntaSeguridad : ITimeStamp
    {
        public long Id { get; set; }

        public string Pregunta { get; set; } = null!;
        public bool EstaActivo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<PreguntaSeguridadUsuario> PreguntaSeguridadUsuario { get; set; } = [];
    }
}
