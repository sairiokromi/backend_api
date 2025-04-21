using FluentValidation;
using FluentValidation.AspNetCore;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Mappings;
using Kromi.Application.Services;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;

namespace Kromi.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");
            services.AddMapster();
            // Configure Mapster
            MapsterConfig.Configure();
            AddServices(services);
            ConfigureValidations(services);
            return services;
        }

        #region Services
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISucursalService, SucursalService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
        }

        #endregion Services

        #region Validations

        public static void ConfigureValidations(IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<CambioContrasenaAdminRequest>()
            .AddValidatorsFromAssemblyContaining<CambioContrasenaRequest>()
            .AddValidatorsFromAssemblyContaining<CambioStatusRequest>()
            .AddValidatorsFromAssemblyContaining<ModificarUsuarioRequest>()
            .AddValidatorsFromAssemblyContaining<RegistrarUsuarioRequest>()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion Validations
    }
}
