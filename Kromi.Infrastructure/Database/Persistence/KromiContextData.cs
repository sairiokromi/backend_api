using Kromi.Domain.Entities;
using Kromi.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kromi.Infrastructure.Database.Persistence
{
    public class KromiContextData
    {
        protected KromiContextData()
        {
        }

        public static async Task LoadDataAsync(
            KromiContext context,
            UserManager<KromiUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            try
            {
                var logger = loggerFactory.CreateLogger<KromiContextData>();
                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole(ERoles.ADMINISTRADOR));
                }

                if (!userManager.Users.Any())
                {
                    var usuarioAdmin = new KromiUser()
                    {
                        Nombres = "Admin",
                        Email = "Admin1@kromi.com",
                        UserName = "Admin1",
                        EstaActivo = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        Foto = "https://cdn-icons-png.flaticon.com/512/6596/6596121.png"
                    };
                    var result = await userManager.CreateAsync(usuarioAdmin, "Admin1@kromi.com");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(usuarioAdmin, ERoles.ADMINISTRADOR);
                    else
                        logger.LogError("Error creando usuario en migracion {@Errors}", result.Errors);
                }

                if (!context.PreguntasSeguridad.Any())
                {
#if DEBUG
                    var preguntasData = await File.ReadAllTextAsync("../Kromi.Infrastructure/Data/preguntas.json");
#else
var preguntasData = await File.ReadAllTextAsync("./Data/preguntas.json");
#endif

                    var preguntas = JsonSerializer.Deserialize<List<PreguntaSeguridad>>(preguntasData);
                    await context.PreguntasSeguridad!.AddRangeAsync(preguntas!);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<KromiContextData>();
                logger.LogError(ex, "Error en migracion");
            }
        }
    }
}
