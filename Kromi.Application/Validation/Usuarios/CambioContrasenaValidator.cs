using FluentValidation;
using Kromi.Application.Data.Dto.Usuarios;

namespace Kromi.Application.Validation.Usuarios
{
    public class CambioContrasenaValidator : AbstractValidator<CambioContrasenaRequest>
    {
        public CambioContrasenaValidator()
        {
            RuleFor(f => f.Nueva).NotEmpty();
            RuleFor(f => f.ConfirmacionNueva).NotEmpty();
            RuleFor(f => f.Actual).NotEmpty();
            RuleFor(c => c.Nueva).Equal(c => c.ConfirmacionNueva);
        }
    }
}
