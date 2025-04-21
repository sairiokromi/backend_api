using FluentResults;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Sucursales;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;
using Microsoft.AspNetCore.Mvc;

namespace Kromi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalesController : ControllerBase
    {
        private readonly ISucursalService _service;
        private readonly ILogger<SucursalesController> _logger;

        public SucursalesController(ISucursalService service, ILogger<SucursalesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el listado de sucursales paginado
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filters"></param>
        /// <param name="sort"></param>
        /// <returns>Un listado paginado de unidades segun los criterios de filtrado y ordenamiento</returns>
        [HttpGet("Paginado", Name = "SucursalesList")]
        [ProducesResponseType<PagedList<SucursalDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Paginado([FromQuery] PaginationQuery query, [FromQuery] GenericBaseFilterQuery filters, [FromQuery] GenericSortQuery sort)
        {
            try
            {
                var unidades = await _service.Listado(query, filters, sort);
                return Ok(unidades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de sucursales");
                return BadRequest(Result.Fail("Error al obtener el listado de sucursales").Errors);
            }
        }

        /// <summary>
        /// Cambia el estado de una sucursal
        /// </summary>
        /// <param name="id"></param>
        /// <returns>indiica si fue o no realizado el cambio de estatus</returns>
        [HttpPost("CambiarStatus/{id}", Name = "CambiarStatusSucursal")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CambiarStatus(long id)
        {
            try
            {
                var result = await _service.CambiarStatus(id);
                if (result.IsSuccess)
                    return Accepted();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar modificar Sucursal");
                return BadRequest(Result.Fail("Error al intentar modificar Sucursal").Errors);
            }
        }

        [HttpGet("Listado", Name = "ListadoSucursales")]
        [ProducesResponseType<List<SucursalDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<List<IError>>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Listado()
        {
            try
            {
                var result = await _service.ListadoActivos();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar obtener listado de sucursales");
                return BadRequest(Result.Fail("Error al intentar obtener listado de sucursales").Errors);
            }
        }
    }
}
