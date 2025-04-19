using FluentValidation;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Mappings;
using Kromi.Application.Services;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Kromi.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            /* services.AddScoped<IUnitOfWork, UnitOfWork>();
             services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));*/

            //services.AddTransient<IAuthService, AuthService>();
            //services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            //services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

            //services.AddDefaultIdentity<KromiUser>(
            //        options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<KromiContext>();
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");
            services.AddMapster();
            // Configure Mapster
            MapsterConfig.Configure();
            AddServices(services);
            return services;
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
