using FluentResults;
using Kromi.Domain.Entities;
using Kromi.Infrastructure.Database.Persistence;
using Kromi.Infrastructure.SettingsModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Kromi.Api.Configurations
{
    public static class ApplicationConfig
    {
        #region Cors
        public static void ConfigureCors(this WebApplicationBuilder builder, string corsName)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsName, builder => builder
                                   .SetIsOriginAllowed(_ => true)
                                   .AllowAnyMethod()
                                   .AllowCredentials()
                                   .AllowAnyHeader());
            });
        }
        #endregion

        #region Seguridad
        public static void ConfigureSecurity(this WebApplicationBuilder builder)
        {
            IdentityBuilder identityBuilder = builder.Services.AddIdentityCore<KromiUser>();
            identityBuilder = new IdentityBuilder(identityBuilder.UserType, identityBuilder.Services);

            identityBuilder.AddRoles<IdentityRole>().AddDefaultTokenProviders();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<KromiUser, IdentityRole>>();

            identityBuilder.AddEntityFrameworkStores<KromiContext>();
            identityBuilder.AddSignInManager<SignInManager<KromiUser>>();

            //soporte para creacion de los datetimes
            builder.Services.AddSingleton(TimeProvider.System);

            var jwtData = new JwtSettings();
            builder.Configuration.Bind("JwtSettings", jwtData);
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.Authority = jwtData.Issuer;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtData.Issuer,
                        ValidAudience = jwtData.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtData.SigningKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
        #endregion

        #region Controladores
        public static void ConfigureControlador(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                      .SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage);
                    var error = Result.Fail(errors);
                    return new BadRequestObjectResult(error.Errors);
                };
            });
        }
        #endregion

        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kromi Api",
                    Version = "v1",
                    Description = "Api kromi",
                    Contact = new OpenApiContact { Email = "sairio20@gmail.com", Name = "Sairio A Peña P" }
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, ["Bearer"] }
                };

                c.AddSecurityRequirement(securityRequirement);
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            builder.Host.UseSerilog((ctx, lc) => lc
                .Enrich.WithProperty("Environment", environment)
                .Enrich.WithCorrelationId()
                .Enrich.WithClientIp()
                .Enrich.WithThreadName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .Enrich.WithRequestHeader("User-Agent")
                .WriteTo.Async(a => a.File("Log/kromi.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, rollingInterval: RollingInterval.Day))
                .WriteTo.Debug());
        }

        public static async Task Migrar(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = service.GetRequiredService<KromiContext>();
                var usuarioManager = service.GetRequiredService<UserManager<KromiUser>>();
                var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
                await context.Database.MigrateAsync();
                await KromiContextData.LoadDataAsync(context, usuarioManager, roleManager, loggerFactory);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Error en la migracion");
            }
        }
    }
}
