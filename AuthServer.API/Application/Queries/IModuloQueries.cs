using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public interface IModuloQueries
    {
        Task<dynamic> ListarAsync();

        Task<IEnumerable<OpcionTreeListDTO>> ListarOpcionUsuarioAsync(int moduloId);

        Task<IEnumerable<AutorizacionTreeListDTO>> ListarAutorizacionUsuarioAsync(int empresaId, string usuarioId, int moduloId);

        Task<dynamic> VerificarAccesoAsync(string viewType);

        Task<dynamic> AccesoAccionAsync(int opcionId);
    }
}