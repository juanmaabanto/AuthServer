using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Models;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public interface IEmpresaRepository : IDisposable
    {
        Task<bool> ActualizarPrincipalAsync(int empresaId);

        Task<Empresa> ObtenerAsync(int empresaId);

        Task AsignarEmpresasAsync(string usuarioId, IEnumerable<UsuarioEmpresa> items);
    }
}