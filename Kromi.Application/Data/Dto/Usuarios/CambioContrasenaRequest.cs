namespace Kromi.Application.Data.Dto.Usuarios
{
    public class CambioContrasenaRequest
    {
        public string Actual { get; set; } = null!;
        public string Nueva { get; set; } = null!;
        public string ConfirmacionNueva { get; set; } = null!;
    }
}
