namespace Kromi.Application.Data.Dto.Usuarios
{
    public class CambioContrasenaAdminRequest
    {
        public string CodigoUsuario { get; set; } = null!;
        public string Nueva { get; set; } = null!;
        public string ConfirmacionNueva { get; set; } = null!;
    }
}
