using FluentResults;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Auth;
using Kromi.Domain.Entities;
using Kromi.Domain.Models;
using Kromi.Infrastructure.Contracts.Identity;
using Kromi.Infrastructure.Database.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Kromi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<KromiUser> _userManager;
        private readonly SignInManager<KromiUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly KromiContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<KromiUser> userManager,
            SignInManager<KromiUser> signInManager,
            IJwtService jwtService,
            KromiContext context,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        public async Task DeleteUserRefreshTokens(string username, string refreshToken)
        {
            _logger.LogWarning("Intento de eliminar refresh token {0}", username);
            var item = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usuario {0} elimina token de refrescamiento", username);
            }
        }

        public Task<UserRefreshTokens?> GetSavedRefreshTokens(string username, string refreshToken)
            => _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive);

        public async Task<Result<AuthResponse>> Login(AuthLoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username)
                ?? await _userManager.FindByEmailAsync(request.Username);
            if (user is null)
            {
                _logger.LogWarning("Intento fallido de login con usuario {0}", request.Username);
                return Result.Fail("El usuario no existe, verifique sus credenciales");
            }

            if (await _userManager.GetAccessFailedCountAsync(user) >= 3)
            {
                user.EstaActivo = false;
                await _userManager.UpdateAsync(user);
                _logger.LogWarning("Intento fallido de login con usuario {0}, excedio numeros de intentos de login", request.Username);
                return Result.Fail("El usuario exedio el numero permitido de intentos de login, por favor contacte con el administrador");
            }

            if (!user.EstaActivo)
            {
                _logger.LogWarning("Intento fallido de login con usuario {0}, usuario inhabilitado", request.Username);
                return Result.Fail("El usuario esta bloqueado, contacte al administrador");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Intento fallido de login con usuario {0}, credenciales erroneas", request.Username);
                _logger.LogWarning("Incrementando numero de intentos fallidos");
                await _userManager.AccessFailedAsync(user);

                return Result.Fail("Las credenciales del usuario son erroneas");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            await SaveRefreshToken(user, refreshToken);

            var AuthResponse = new AuthResponse
            {
                Apellidos = user.Apellidos!,
                Email = user.Email!,
                EstaActivo = user.EstaActivo,
                Ficha = user.Ficha,
                Firma = user.Firma,
                Foto = user.Foto,
                Nombres = user.Nombres,
                RefreshToken = refreshToken,
                Roles = roles,
                UserName = user.UserName!
            };
            AuthResponse.Token = _jwtService.GenerateUserAccessToken(user, roles);
            AuthResponse.RefreshToken = refreshToken;
            AuthResponse.Roles = roles;
            _logger.LogInformation("Login ok de usuario {0}", request.Username);
            await _userManager.ResetAccessFailedCountAsync(user);
            return Result.Ok(AuthResponse);
        }

        public async Task<Result<RefreshResponse>> RefreshToken(RefreshResponse request)
        {
            _logger.LogInformation("Se obtiene claim principal");
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.Token);
            var username = principal.Claims.First(f => f.Type == JwtRegisteredClaimNames.Name)?.Value;

            var savedRefreshToken = await GetSavedRefreshTokens(username!, request.RefreshToken);

            if (savedRefreshToken is null || savedRefreshToken.RefreshToken != request.RefreshToken)
            {
                _logger.LogError("Usuario {0} imposible obtener refresh token", username);
                return Result.Fail("Imposible obtener refresh token");
            }

            var user = await _userManager.FindByNameAsync(username!);

            if (user is null)
            {
                _logger.LogWarning("Intento fallido de refresh con usuario {0}", username);
                return Result.Fail("El usuario no existe, verifique sus credenciales");
            }

            if (!user.EstaActivo)
            {
                _logger.LogWarning("Intento fallido de refresh con usuario {0}, usuario inhabilitado", username);
                return Result.Fail("El usuario esta bloqueado, contacte al administrador");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var token = _jwtService.GenerateUserAccessToken(user, roles);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Error generando token a usuario {0}", username);
                return Result.Fail("Error generando token, contacte al administrador");
            }

            await DeleteUserRefreshTokens(username!, request.RefreshToken);
            await SaveRefreshToken(user, refreshToken);

            return Result.Ok(new RefreshResponse
            {
                Token = token,
                RefreshToken = refreshToken,
            });
        }

        private async Task SaveRefreshToken(KromiUser user, string token)
        {
            var refresh = new UserRefreshTokens
            {
                IsActive = true,
                RefreshToken = token,
                UserName = user.UserName!
            };
            _logger.LogInformation("Guardando refresh token de usuario {0}", user.Email);
            await _context.UserRefreshTokens.AddAsync(refresh);
            await _context.SaveChangesAsync();
        }
    }
}
