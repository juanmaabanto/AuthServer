using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public interface IEmpresaQueries
    {
        Task<IEnumerable<EmpresaListDTO>> ListarPorSesionAsync();
        
    }
}