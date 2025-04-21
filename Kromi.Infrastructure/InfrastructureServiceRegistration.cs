using Kromi.Infrastructure.Contracts.Identity;
using Kromi.Infrastructure.Database.Audit;
using Kromi.Infrastructure.Database.Persistence;
using Kromi.Infrastructure.SettingsModels;
using Kromi.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kromi.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Base de datos
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<KromiContext>((provider, options) =>
            {
                options.UseSqlServer(connectionString);
                var interceptor = provider.GetRequiredService<AuditableInterceptor>();
                options.EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .AddInterceptors(interceptor);
            });
            #endregion

            #region Services
            services.AddSingleton<AuditableInterceptor>();
            services.AddTransient<IJwtService, JwtService>();
            #endregion

            #region Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<MailSmtpSettings>(configuration.GetSection("MailSmtpSettings"));
            #endregion

            return services;
        }
    }
}
