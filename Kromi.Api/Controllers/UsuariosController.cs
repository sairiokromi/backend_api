using FluentResults;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Domain.Entities.StoredProcedures;
using Microsoft.AspNetCore.Mvc;

namespace Kromi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Cambia el status de un usuario
        /// </summary>
        /// <param name="request">indeitificador del usuario</param>
        /// <returns>valor vacio</returns>
        [HttpPost("CambiarStatus", Name = "CambiarStatusUsuario")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CambiarStatus([FromBody] CambioStatusRequest request)
        {
            try
            {
                var result = await _usuarioService.CambiarStatus(request);
                if (result.IsSuccess)
                    return Accepted();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar modificar el usuario");
                return BadRequest(Result.Fail("Error al intentar modificar el usuario").Errors);
            }
        }

        /// <summary>
        /// Valida si un email esta registrado previamente
        /// </summary>
        /// <param name="email">email a verificar</param>
        /// <returns>booleano</returns>
        [HttpGet("VerifyEmail", Name = "verifyEmailUser")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        [ProducesResponseType<bool>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyEmail(string email)
        {
            try
            {
                var result = await _usuarioService.VerificarEmail(email);
                return Ok(result.IsSuccess);
            }
            catch (Exception)
            {
                return Ok(false);
            }
        }

        /// <summary>
        /// Valida si un nombre de usuario esta registrado previamente
        /// </summary>
        /// <param name="username">nombre de usuario a verificar</param>
        /// <returns>booleano</returns>
        [HttpGet("VerifyUsername", Name = "VerifyUsernameUser")]
        [ProducesResponseType<bool>(StatusCodes.Status200OK)]
        [ProducesResponseType<bool>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyUsername(string username)
        {
            try
            {
                var result = await _usuarioService.VerificarNombreUsuario(username);
                return Ok(result.IsSuccess);
            }
            catch (Exception)
            {
                return Ok(false);
            }
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="request">Campos para registrar el usuario</param>
        /// <returns>status creado o bad request con errores</returns>
        [HttpPost("Registrar", Name = "RegistrarUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Registrar([FromBody] RegistrarUsuarioRequest request)
        {
            try
            {
                var resp = await _usuarioService.RegistrarUsuario(request);
                if (resp.IsSuccess) return Created();
                else return BadRequest(resp.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando el usuario");
                return BadRequest(Result.Fail("Error registrando el usuario").Errors);
            }
        }

        /// <summary>
        /// Modifica un usuario existente
        /// </summary>
        /// <param name="request">Campos para registrar el usuario</param>
        /// <returns>status creado o bad request con errores</returns>
        [HttpPost("Actualizar", Name = "ActualizarUsuario")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ActualizarUsuario([FromBody] ModificarUsuarioRequest request)
        {
            try
            {
                var resp = await _usuarioService.ModificarUsuario(request);
                if (resp.IsSuccess) return Accepted();
                else return BadRequest(resp.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el usuario");
                return BadRequest(Result.Fail("Error actualizando el usuario").Errors);
            }
        }

        [HttpPost("CambiarContrasenaAdmin", Name = "CambiarContrasenaAdmin")]
        public async Task<ActionResult> CambiarContrasenaAdmin([FromBody] CambioContrasenaAdminRequest request)
        {
            try
            {
                var resp = await _usuarioService.CambiarContrasenaAdmin(request);
                if (resp.IsSuccess) return Created();
                else return BadRequest(resp.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cambiando la contraseña el usuario");
                return BadRequest(Result.Fail("Error cambiando la contraseña el usuario").Errors);
            }
        }

        /// <summary>
        /// Devuelve un listado de usuario con perfil evaluador
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListadoEvaluadores")]
        [ProducesResponseType<List<GenericStringString>>(StatusCodes.Status200OK)]
        public async Task<ActionResult> Evaluadores()
        {
            return Ok(await _usuarioService.ObtenerEvaluadoresSelect());
        }
    }
}
