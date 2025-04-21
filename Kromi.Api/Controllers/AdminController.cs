using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;
using Microsoft.AspNetCore.Mvc;

namespace Kromi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUsuarioService usuarioService, ILogger<AdminController> logger, IAdminService adminService)
        {
            _usuarioService = usuarioService;
            _logger = logger;
            _adminService = adminService;
        }

        /// <summary>
        /// Obtiene el listado de usuarios
        /// </summary>
        /// <returns>Una lista de usuarios</returns>
        [HttpGet("ListadoUsuarios")]
        [ProducesResponseType(typeof(PagedList<UsuarioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListadoUsuarios([FromQuery] PaginationQuery query, [FromQuery] GenericBaseFilterQuery filters, [FromQuery] GenericSortQuery sort)
        {
            try
            {
                var usuarios = await _usuarioService.ListadoUsuarios(query, filters, sort);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de usuarios");
                return StatusCode(StatusCodes.Status400BadRequest, "Error al obtener el listado de usuarios");
            }
        }

        /// <summary>
        /// Obtiene el listado de usuarios
        /// </summary>
        /// <returns>Una lista de usuarios</returns>
        [HttpGet("ListadoRoles")]
        [ProducesResponseType(typeof(PagedList<UsuarioDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public IActionResult ListadoRoles()
        {
            try
            {
                var usuarios = _adminService.ListadoRoles();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de usuarios");
                return StatusCode(StatusCodes.Status400BadRequest, "Error al obtener el listado de usuarios");
            }
        }
    }
}
