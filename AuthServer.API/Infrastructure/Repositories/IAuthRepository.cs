using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.SeedWork;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public interface IAuthRepository : IRepository<Usuario>
    {
        #region Aplicacion Cliente

        Task<UsuarioAplicacionCliente> AgregarUsuarioAplicacionAsync(string usuarioId, string aplicacionId);
        Task<AplicacionCliente> GetAplicacionClienteAsync(string aplicacionClienteId, CancellationToken cancellationToken = default(CancellationToken));
        Task<dynamic> GetUsuarioAplicacionAsync(string usuario, string applicacionClienteId);

        #endregion

        #region Empresa

        Task<UsuarioEmpresa> CompanyData(int empresaId, string usuarioId, string usuario);

        IEnumerable<Empresa> GetCompanies(string userId, string userName);

        Task<int> GetEmpresaIdAsync(string usuarioId);
        Task<int> GetEmpresaIdAsync(string usuarioId, string codigo);

        #endregion

        #region Usuario

        Task<Usuario> AutenticarAsync(string nombreUsuario, string clave, string aplicacionClienteId);
        Task<bool> CambiarClaveAsync(string nombreUsuario, string clave, string nuevaClave);
        Task<string> GetRolAsync(string usuarioId);
        Task<dynamic> GetUsuarioAsync(string nombreUsuario);
        Task<Usuario> GetUsuarioPorIdAsync(string usuarioId);
        Task<int> SaveAuthorizationAsync(List<Dictionary<string, object>> lst, string userId, byte companyId, string userName);
        bool IsAuthorize(string userId, int companyId, string controller, string action, string aplicacionClienteId);

        #endregion
    }
}