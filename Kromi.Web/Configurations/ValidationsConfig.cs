using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace Kromi.Web.Configurations
{
    public static class ValidationsConfig
    {
        public static void ConfigureValidations(this WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //builder.Services.AddValidatorsFromAssemblyContaining<LoginUserValidation>();
            //builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();
            //builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidation>();
            //builder.Services.AddValidatorsFromAssemblyContaining<AdminChangePasswordValidation>();
            //builder.Services.AddValidatorsFromAssemblyContaining<DetalleRetencionValidation>();
        }
    }
}
