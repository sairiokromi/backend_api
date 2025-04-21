using FluentValidation;
using Kromi.Application.Data.Utils;

namespace Kromi.Application.Validation.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string?> Base64Image<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            var base64Properties = new Base64FileProperties();
            return ruleBuilder
                       .MinimumLength(10)
                       .Must(val =>
                       {
                           var result = base64Properties.GetBase64FileProperties(val);
                           return result == AttachmentType.Png
                           || result == AttachmentType.Jpg
                           || result == AttachmentType.Gif;
                       });
        }

        public static IRuleBuilderOptions<T, string?> PhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                       .MinimumLength(10)
                       .MaximumLength(20)
                       .Matches(@"^\+\d{1,20}$");
        }

        public static IRuleBuilderOptions<T, string?> GuidValidation<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {

            return ruleBuilder
                       .MinimumLength(10)
                       .Must(val =>
                       {
                           var isValid = Guid.TryParse(val, out _);
                           return isValid;
                       });
        }

        public static IRuleBuilderOptions<T, string?> StrongPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                       .MinimumLength(6)
                       .Matches(@"[A-Z]+").WithMessage("Debe tener al menos una letra en mayuscula.")
                       .Matches(@"[a-z]+").WithMessage("Debe tener al menos una letra en minuscula.")
                       .Matches(@"[0-9]+").WithMessage("Debe tener al menos tener un numero.")
                       .Matches(@"[\#\.\,\:\;\*\%\$\@\!]+").WithMessage("Debe tener al menos uno de los siguientes caracteres #.,:;*%$@!");
        }
    }
}
