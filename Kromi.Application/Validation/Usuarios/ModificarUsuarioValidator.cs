using FluentValidation;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Validation.Extensions;

namespace Kromi.Application.Validation.Usuarios
{
    public class ModificarUsuarioValidator : AbstractValidator<ModificarUsuarioRequest>
    {
        public ModificarUsuarioValidator()
        {
            RuleFor(f => f.Email).NotEmpty().EmailAddress();
            RuleFor(d => d.Role).NotEmpty();
            RuleFor(d => d.UserName).NotEmpty().MinimumLength(4);
            RuleFor(d => d.Nombres).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(d => d.Apellidos).MinimumLength(3).MaximumLength(50);
            RuleFor(d => d.Telefono).PhoneNumber().When(d => !string.IsNullOrWhiteSpace(d.Telefono));
            RuleFor(d => d.Ficha).MaximumLength(20);
            RuleFor(d => d.Id).NotEmpty().GuidValidation();
            RuleFor(d => d.Foto)
                .Base64Image()
                .WithMessage("El base64 no es valido para las imagenes permitidas, solo se permiten JPG, PNG, GIF")
                .When(reg => !string.IsNullOrWhiteSpace(reg.Foto));
            RuleFor(d => d.Firma)
                .Base64Image()
                .WithMessage("El base64 no es valido para las imagenes permitidas, solo se permiten JPG, PNG, GIF")
                .When(reg => !string.IsNullOrWhiteSpace(reg.Firma));
        }
    }
}
