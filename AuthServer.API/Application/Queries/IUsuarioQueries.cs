using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public interface IUsuarioQueries
    {
        Task<(int total, IEnumerable<UsuarioListDTO> data)> ListarAsync(string nombre, string sort, int pageSize, int start, CancellationToken cancellationToken);
        Task<UsuarioDTO> ObtenerAsync(string usuarioId);
        Task<byte[]> ObtenerBytesAsync(string nombre);
        Task<PerfilDTO> ObtenerPerfilAsync();
        Task<PerfilDTO> ObtenerPerfilAsync(string usuarioId);
    }
}