using FluentResults;
using Kromi.Application.Data.Dto.Sucursales;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;

namespace Kromi.Application.Contracts.Services
{
    public interface ISucursalService
    {
        Task<PagedList<SucursalDto>> Listado(PaginationQuery query, GenericBaseFilterQuery filters, GenericSortQuery sort);
        Task<Result> CambiarStatus(long id);
        Task<List<SucursalDto>> ListadoActivos();
    }
}
