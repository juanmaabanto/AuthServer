using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.CrossCutting.LoggingEvent;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        #region Variables

        private readonly AuthContext _ctx;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public EmpresaRepository(AuthContext context, IIdentityService identityService, ILogger logger)
        {
            _ctx = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Metodos

        public async Task<bool> ActualizarPrincipalAsync(int empresaId)
        {
            string usuarioId = _identityService.UsuarioId;
            string usuario = _identityService.Usuario;

            try
            {
                var current = _ctx.Set<UsuarioEmpresa>().Where(u => u.UsuarioId == usuarioId && u.Principal).FirstOrDefault();

                if(current == null || current.EmpresaId == empresaId)
                {
                    return true;
                }

                current.Principal = false;
                _ctx.Set<UsuarioEmpresa>().Update(current);

                var usuarioEmpresa = await _ctx.Set<UsuarioEmpresa>().FindAsync(usuarioId, empresaId);

                if(usuarioEmpresa == null)
                {
                    throw new AuthDomainException("El usuario no tiene acceso a la empresa seleccionada.");
                }

                usuarioEmpresa.Principal = true;
                _ctx.Set<UsuarioEmpresa>().Update(current);

                await _ctx.SaveChangesAsync();

                return true;
            }
            catch (AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException("Ocurrio un error al obtener accesos del cliente.", result.EventLogId);
            }
        }

        public async Task<Empresa> ObtenerAsync(int empresaId)
        {
            return await _ctx.Set<Empresa>().FindAsync(empresaId);
        }

        public async Task AsignarEmpresasAsync(string usuarioId, IEnumerable<UsuarioEmpresa> items)
        {
            var currents = _ctx.Set<UsuarioEmpresa>().Where(u => u.UsuarioId == usuarioId);

            _ctx.RemoveRange(currents);

            await _ctx.AddRangeAsync(items);
        }

        #endregion

        #region "Dispose"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ctx != null)
                {
                    _ctx.Dispose();
                }
            }
        }

        #endregion

    }
}