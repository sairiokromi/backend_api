namespace Kromi.Application.Data.Dto.Sucursales
{
    public class SucursalDto
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public bool EstaActivo { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
