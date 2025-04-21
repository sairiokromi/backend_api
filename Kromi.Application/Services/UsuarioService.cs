using FluentResults;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;
using Kromi.Application.Data.Utils;
using Kromi.Domain.Entities;
using Kromi.Domain.Entities.StoredProcedures;
using Kromi.Domain.Extensions;
using Kromi.Infrastructure.Contracts.Identity;
using Kromi.Infrastructure.Database.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kromi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly KromiContext _context;
        private readonly UserManager<KromiUser> _userManager;
        private readonly IJwtService _jwthService;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IHostEnvironment _env;
        private readonly string _baseUrl;

        public UsuarioService(
            KromiContext context,
            UserManager<KromiUser> userManager,
            ILogger<UsuarioService> logger,
            IConfiguration configuration,
            IHostEnvironment env,
            IJwtService jwthService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _env = env;
            _baseUrl = configuration.GetValue<string>("BaseUrl")!;
            _jwthService = jwthService;
        }

        public async Task<Result> CambiarContrasena(CambioContrasenaRequest request)
        {
            string? usuario = _jwthService.GetSessionUserId();
            var user = await _userManager.FindByIdAsync(usuario!);
            _logger.LogInformation("Obteniendo usuario");
            if (user is null)
            {
                _logger.LogWarning("Problemas obteniendo datos de usuario");
                return Result.Fail("Problemas obteniendo datos de usuario");
            }
            var resp = await _userManager.ChangePasswordAsync(user, request.Actual, request.Nueva);
            if (!resp.Succeeded)
            {
                _logger.LogWarning("Ocurrio un error modificando su nueva contraseña, verifique que la actual sea correcta o la nueva cumpla con los criterios de seguridad");
                return Result.Fail("Ocurrio un error modificando su nueva contraseña, verifique que la actual sea correcta o la nueva cumpla con los criterios de seguridad");
            }
            return Result.Ok();
        }

        public async Task<Result> CambiarContrasenaAdmin(CambioContrasenaAdminRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.CodigoUsuario);
            _logger.LogInformation("Obteniendo usuario");
            if (user is null)
            {
                _logger.LogWarning("Problemas obteniendo datos de usuario, el usuario no existe");
                return Result.Fail("Problemas obteniendo datos de usuario, el usuario no existe");
            }
            _logger.LogInformation("generando token para cambio de contraseña");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resp = await _userManager.ResetPasswordAsync(user, token, request.Nueva);
            if (!resp.Succeeded)
            {
                _logger.LogWarning("Ocurrio un error modificando su nueva contraseña, verifique que la nueva contraseña cumpla con los criterios de seguridad");
                return Result.Fail("Ocurrio un error modificando su nueva contraseña, verifique que la nueva contraseña cumpla con los criterios de seguridad");
            }
            return Result.Ok();
        }

        public async Task<Result> CambiarFoto(string foto)
        {
            var user = await _userManager.FindByIdAsync(_jwthService.GetSessionUserId()!);
            if (user is null)
            {
                _logger.LogWarning("Problemas obteniendo datos de usuario, el usuario no existe");
                return Result.Fail("Problemas obteniendo datos de usuario, el usuario no existe");
            }
            if (!string.IsNullOrWhiteSpace(foto))
            {
                var savedfoto = await GuardarImagen(foto);
                user.Foto = $"{_baseUrl}Files/Images/{savedfoto}";
                await _userManager.UpdateAsync(user);
            }

            return Result.Ok();
        }

        public async Task<Result> CambiarStatus(CambioStatusRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.CodigoUsuario);
            _logger.LogInformation("Obteniendo usuario");
            if (user is null)
            {
                _logger.LogWarning("Problemas obteniendo datos de usuario, el usuario no existe");
                return Result.Fail("Problemas obteniendo datos de usuario, el usuario no existe");
            }
            user.EstaActivo = !user.EstaActivo;
            var resp = await _userManager.UpdateAsync(user);
            if (!resp.Succeeded)
            {
                _logger.LogWarning("Problemas actualizando los datos del usuario");
                return Result.Fail("Problemas actualizando los datos del usuario");
            }

            if (user.EstaActivo)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return Result.Ok();
        }

        public async Task<PagedList<UsuarioDto>> ListadoUsuarios(PaginationQuery query, GenericBaseFilterQuery filters, GenericSortQuery? sort)
        {
            var data = _context.KromiUsers
                .Include(i => i.Sucursal)
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                string search = filters.Search.ToUpper();
                data = data.Where(w => w.NormalizedUserName != null && w.NormalizedUserName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                            w.NormalizedEmail != null && w.NormalizedEmail.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                            w.Nombres != null && w.Nombres.ToUpper().Contains(search) ||
                            w.Apellidos != null && w.Apellidos.ToUpper().Contains(search) ||
                            w.PhoneNumber != null && w.PhoneNumber.Contains(search)
                        );
            }

            if (sort != null && !string.IsNullOrWhiteSpace(sort.Field))
            {
                data = data.OrderByDynamic(sort.Field, sort.Order);
            }
            var paged = await PagedList<KromiUser>.ToPagedListAsync(data, query.PageNumber, query.PageSize);
            List<UsuarioDto> mapped = [];
            foreach (var s in paged.Items)
            {
                var user = new UsuarioDto
                {
                    Id = s.Id,
                    UpdatedAt = s.UpdatedAt,
                    BloqueadoEl = s.BloqueadoEl,
                    CreatedAt = s.CreatedAt,
                    Email = s.Email ?? "",
                    EstaActivo = s.EstaActivo,
                    Foto = s.Foto,
                    Nombres = s.Nombres,
                    Apellidos = s.Apellidos,
                    Ficha = s.Ficha,
                    Telefono = s.PhoneNumber,
                    Firma = s.Firma,
                    UserName = s.UserName ?? "",
                    SucursalId = s.Sucursal?.Id,
                    Sucursal = s.Sucursal?.Nombre,
                    Roles = await _userManager.GetRolesAsync(s)
                };
                mapped.Add(user);
            }
            return new PagedList<UsuarioDto>(mapped, paged.TotalCount, paged.CurrentPage, paged.PageSize);
        }

        public async Task<Result> ModificarUsuario(ModificarUsuarioRequest request)
        {
            _logger.LogInformation("Obteniendo datos de usuario");
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user is null)
            {
                _logger.LogWarning("Se intenta modificar usuario con id inexistente {@Request}", request);
                return Result.Fail("El usuario no existe");
            }

            if (!request.Email.Equals(user.Email))
            {
                _logger.LogInformation("Verificando email de usuario");
                var email = await _userManager.FindByEmailAsync(request.Email);
                if (email is not null)
                {
                    _logger.LogWarning("El email de usuario ya existe {0}", request.Email);
                    return Result.Fail($"El email de usuario ya existe {request.Email}");
                }
            }

            if (!request.UserName.Equals(user.UserName))
            {
                _logger.LogInformation("Verificando nombre de usuario");
                var username = await _userManager.FindByNameAsync(request.UserName);

                if (username is not null)
                {
                    _logger.LogWarning("El nombre de usuario ya existe {0}", request.UserName);
                    return Result.Fail($"El nombre de usuario ya existe {request.UserName}");
                }
            }

            user.Email = request.Email;
            user.UserName = request.UserName;
            user.Nombres = request.Nombres;
            user.Apellidos = request.Apellidos;
            user.PhoneNumber = request.Telefono;
            user.SucursalId = request.SucursalId;
            user.Ficha = request.Ficha;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Modificando usuario {0}", user.UserName);
                var resultUser = await _userManager.UpdateAsync(user);
                if (!resultUser.Succeeded)
                {
                    _logger.LogWarning("Ocurrio un error modificando usuario {@Errors}", resultUser.Errors);
                    await transaction.RollbackAsync();
                    return Result.Fail("Error modificando usuario");
                }

                var role = await _userManager.GetRolesAsync(user);

                if (role is not null)
                {
                    await _userManager.RemoveFromRolesAsync(user, role);
                    _logger.LogInformation("Asignando rol a usuario {0}", user.UserName);
                    var resultRole = await _userManager.AddToRolesAsync(user, request.Role);
                    if (!resultRole.Succeeded)
                    {
                        _logger.LogWarning("Ocurrio un error registrando el rol al usuario {0} {@Errors}", user.UserName, resultUser.Errors);
                        await transaction.RollbackAsync();
                        return Result.Fail("Error asignando rol al usuario");
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.Foto))
                {
                    var foto = await GuardarImagen(request.Foto);
                    user.Foto = $"{_baseUrl}Files/Images/{foto}";
                    await _userManager.UpdateAsync(user);
                }

                if (!string.IsNullOrWhiteSpace(request.Firma))
                {
                    var foto = await GuardarImagen(request.Firma);
                    user.Firma = $"{_baseUrl}Files/Images/{foto}";
                    await _userManager.UpdateAsync(user);
                }

                await transaction.CommitAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando usuario");
                await transaction.RollbackAsync();
            }
            return Result.Fail("Error desconocido registrando usuario");
        }

        public async Task<List<GenericStringString>> ObtenerEvaluadoresSelect()
        {
            const string sql = @"SELECT u.Id, CONCAT(u.Nombres,' ' ,u.Apellidos) as Name
                        FROM [dbo].[AspNetUsers] u
                        INNER JOIN [dbo].[AspNetUserRoles] ur on u.Id = ur.UserId
                        INNER JOIN [dbo].[AspNetRoles] r on ur.RoleId = r.Id
                        WHERE r.NormalizedName = 'EVALUADOR' ORDER BY u.Nombres";

            return await _context.GenericStringString.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<Result> RegistrarUsuario(RegistrarUsuarioRequest request)
        {
            _logger.LogInformation("Verificando nombre de usuario");
            var username = await _userManager.FindByNameAsync(request.UserName);

            if (username is not null)
            {
                _logger.LogWarning("El nombre de usuario ya existe {0}", request.UserName);
                return Result.Fail($"El nombre de usuario ya existe {request.UserName}");
            }

            var email = await _userManager.FindByEmailAsync(request.Email);
            if (email is not null)
            {
                _logger.LogWarning("El email de usuario ya existe {0}", request.Email);
                return Result.Fail($"El email de usuario ya existe {request.Email}");
            }

            var user = new KromiUser
            {
                UserName = request.UserName,
                Email = request.Email,
                EstaActivo = true,
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Ficha = request.Ficha,
                PhoneNumber = request.Telefono,
                SucursalId = request.SucursalId,
            };
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Registrando usuario {0}", user.UserName);
                var resultUser = await _userManager.CreateAsync(user, request.Contrasena);
                if (!resultUser.Succeeded)
                {
                    _logger.LogWarning("Ocurrio un error registrando usuario {@Errors}", resultUser.Errors);
                    await transaction.RollbackAsync();
                    return Result.Fail("Error registrando usuario");
                }

                _logger.LogInformation("Asignando rol a usuario {0}", user.UserName);
                var resultRole = await _userManager.AddToRolesAsync(user, request.Role);

                if (!resultRole.Succeeded)
                {
                    _logger.LogWarning("Ocurrio un error registrando el rol al usuario {0} {@Errors}", user.UserName, resultUser.Errors);
                    await transaction.RollbackAsync();
                    return Result.Fail("Error asignando rol al usuario");
                }

                if (!string.IsNullOrWhiteSpace(request.Foto))
                {
                    var foto = await GuardarImagen(request.Foto);
                    user.Foto = $"{_baseUrl}Files/Images/{foto}";
                    await _userManager.UpdateAsync(user);
                }

                if (!string.IsNullOrWhiteSpace(request.Firma))
                {
                    var foto = await GuardarImagen(request.Firma);
                    user.Firma = $"{_baseUrl}Files/Images/{foto}";
                    await _userManager.UpdateAsync(user);
                }

                await transaction.CommitAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando usuario");
                await transaction.RollbackAsync();
            }
            return Result.Fail("Error desconocido registrando usuario");
        }

        public async Task<Result> VerificarEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null) return Result.Fail("");
            else return Result.Ok();
        }

        public async Task<Result> VerificarNombreUsuario(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is not null) return Result.Fail("");
            else return Result.Ok();
        }

        private async Task<string> GuardarImagen(string foto)
        {
            var base64Properties = new Base64FileProperties();
            var base64Prop = base64Properties.GetBase64FileProperties(foto);
            var name = $"{Guid.NewGuid()}{base64Prop.Extension}";
            byte[] bytes = Convert.FromBase64String(foto);
            var path = Path.Combine(_env.ContentRootPath, "Files", "Images", name);
            await File.WriteAllBytesAsync(path, bytes);
            return name;
        }
    }
}
