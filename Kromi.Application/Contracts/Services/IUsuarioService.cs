using FluentResults;
using Kromi.Application.Data.Dto.Usuarios;
using Kromi.Application.Data.Models;
using Kromi.Application.Data.Models.GenericQueries;
using Kromi.Domain.Entities.StoredProcedures;

namespace Kromi.Application.Contracts.Services
{
    public interface IUsuarioService
    {
        Task<PagedList<UsuarioDto>> ListadoUsuarios(PaginationQuery query, GenericBaseFilterQuery filters, GenericSortQuery? sort);
        Task<Result> CambiarStatus(CambioStatusRequest request);
        Task<Result> CambiarContrasena(CambioContrasenaRequest request);
        Task<Result> CambiarContrasenaAdmin(CambioContrasenaAdminRequest request);
        Task<Result> VerificarEmail(string email);
        Task<Result> VerificarNombreUsuario(string username);
        Task<Result> RegistrarUsuario(RegistrarUsuarioRequest request);
        Task<Result> ModificarUsuario(ModificarUsuarioRequest request);
        Task<Result> CambiarFoto(string foto);
        Task<List<GenericStringString>> ObtenerEvaluadoresSelect();
    }
}
