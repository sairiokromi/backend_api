using FluentResults;
using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Sucursales;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;
using Kromi.Domain.Extensions;
using Kromi.Infrastructure.Database.Extensions;
using Kromi.Infrastructure.Database.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kromi.Application.Services
{
    public class SucursalService : ISucursalService
    {
        private readonly KromiContext _context;
        private readonly ILogger<SucursalService> _logger;

        public SucursalService(KromiContext context,
            ILogger<SucursalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> CambiarStatus(long id)
        {
            var sucursal = await _context.Sucursal.FindAsync(id);

            if (sucursal is null)
            {
                _logger.LogError("Sucursal no encontrada con id {0}", id);
                return Result.Fail("Sucursal no encontrada");
            }

            sucursal.EstaActivo = !sucursal.EstaActivo;
            _context.Sucursal.Update(sucursal);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }

        public Task<PagedList<SucursalDto>> Listado(PaginationQuery query, GenericBaseFilterQuery filters, GenericSortQuery sort)
        {
            var sucursales = _context.Sucursal
                .Select(s => new SucursalDto
                {
                    Id = s.Id,
                    Codigo = s.Codigo,
                    Direccion = s.Direccion,
                    EstaActivo = s.EstaActivo,
                    UpdatedAt = s.UpdatedAt,
                    CreatedAt = s.CreatedAt,
                    Nombre = s.Nombre
                })
                .AsNoTracking();
            var predicate = PredicateBuilder.True<SucursalDto>();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                string search = filters.Search.ToUpper();
                predicate = predicate.And(w => w.Codigo.Contains(search) ||
                                               w.Nombre.Contains(search));
            }
            if (sort is not null && !string.IsNullOrWhiteSpace(sort.Field))
            {
                sucursales = sucursales.OrderByDynamic(sort.Field, sort.Order);
            }

            return PagedList<SucursalDto>.ToPagedListAsync(sucursales.Where(predicate), query.PageNumber, query.PageSize);
        }

        public Task<List<SucursalDto>> ListadoActivos()
        => _context.Sucursal.Select(s => new SucursalDto
        {
            Id = s.Id,
            Codigo = s.Codigo,
            Direccion = s.Direccion,
            EstaActivo = s.EstaActivo,
            UpdatedAt = s.UpdatedAt,
            CreatedAt = s.CreatedAt,
            Nombre = s.Nombre
        }).Where(w => w.EstaActivo).OrderBy(o => o.Nombre).ToListAsync();
    }
}
