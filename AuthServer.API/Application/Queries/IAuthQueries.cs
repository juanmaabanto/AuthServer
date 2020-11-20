using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Models;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public interface IAuthQueries
    {
       Task<dynamic> GetDatosSesionAsync();

       Task<dynamic> ObtenerUsuariosPorRol(int empresaId, string rol, string usuario);
        Task<dynamic> ObtenerModulosAsync(string usuarioId, string aplicacionClienteId, string usuario);

        Task<dynamic> ObtenerRecursosAsync(string usuarioId, string usuario);

        Task<dynamic> TieneAccesoAplicacionAsync(string usuarioId, int moduloId, string usuario);

        Task<dynamic> ObtenerOpcionAsync(string viewType, string usuario);

        Task<IEnumerable<OpcionDTO>> ObtenerOpcionPorModuloAsync(int moduloId, string usuario);
    }
}