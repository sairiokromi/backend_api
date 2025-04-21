using FluentValidation;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Validation.Extensions;

namespace Kromi.Application.Validation.Usuarios
{
    public class CambioContrasenaAdminValidator : AbstractValidator<CambioContrasenaAdminRequest>
    {
        public CambioContrasenaAdminValidator()
        {
            RuleFor(f => f.CodigoUsuario)
                .NotEmpty().WithName("Codigo de usuario")
                .GuidValidation().WithName("Codigo de usuario").WithMessage("El codigo de usuario no es correcto");
            RuleFor(f => f.Nueva)
                .NotEmpty().WithName("Contraseña")
                .StrongPassword().WithName("Contraseña");
            RuleFor(f => f.ConfirmacionNueva)
                .NotEmpty().WithName("Confirmacion contraseña");
            RuleFor(c => c.Nueva)
                .Equal(c => c.ConfirmacionNueva).WithName("Contraseña");
        }
    }
}
