using FluentValidation;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Validation.Extensions;

namespace Kromi.Application.Validation.Usuarios
{
    public class CambioStatusValidator : AbstractValidator<CambioStatusRequest>
    {
        public CambioStatusValidator()
        {
            RuleFor(f => f.CodigoUsuario)
                .NotEmpty()
                .GuidValidation().WithMessage("El codigo de usuario no es correcto");
        }
    }
}
