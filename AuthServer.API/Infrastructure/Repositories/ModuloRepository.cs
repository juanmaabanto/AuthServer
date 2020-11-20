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
    public class ModuloRepository : IModuloRepository
    {
        #region Variables

        private readonly AuthContext _ctx;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public ModuloRepository(AuthContext context, IIdentityService identityService, ILogger logger)
        {
            _ctx = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Detalle Autorización

        public async Task<IEnumerable<object>> ObtenerAccesoAccionAsync(int opcionId)
        {
            int empresaId = _identityService.EmpresaId;
            string usuarioId = _identityService.UsuarioId;
            string usuario = _identityService.Usuario;
            IEnumerable<object> detail = null;

            try
            {
                if (_identityService.Rol.Equals("Administrador"))
                {
                    var entities = _ctx.Set<DetalleOpcion>().Where(d => d.OpcionId == opcionId && d.Activo);
                    detail = (from d in entities
                              select new
                              {
                                  Nombre = d.NombreAccion
                              }).Distinct();
                }
                else
                {
                    detail = (from d in _ctx.Set<DetalleAutorizacion>()
                        join o in _ctx.Set<DetalleOpcion>() on d.DetalleOpcionId equals o.DetalleOpcionId
                        where o.Activo && d.EmpresaId == empresaId && d.UsuarioId == usuarioId && d.OpcionId == opcionId
                        select new { 
                            Nombre = o.NombreAccion
                         } ).Distinct();
                }
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException("Ocurrio un error al obtener accesos del cliente.", result.EventLogId);
            }

            return detail;
        }

        #endregion

        #region Favoritos

        public async Task<bool> AgregarFavoritoAsync(int opcionId)
        {
            var empresaId = _identityService.EmpresaId;
            var usuarioId = _identityService.UsuarioId;

            try
            {
                var current = await _ctx.Favoritos.FindAsync(empresaId, opcionId, usuarioId);

                if (current == null)
                {
                    await _ctx.Favoritos.AddAsync(new Favorito(){
                        Activo = true,
                        EmpresaId = empresaId,
                        OpcionId = opcionId,
                        UsuarioId = usuarioId
                    });

                    await _ctx.SaveChangesAsync();
                    return true;
                }
                else
                {
                    if (!current.Activo)
                    {
                        current.Activo = true;

                        _ctx.Favoritos.Update(current);
                        await _ctx.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return false;
        }

        public async Task<bool> QuitarFavoritoAsync(int opcionId)
        {
            var empresaId = _identityService.EmpresaId;
            var usuarioId = _identityService.UsuarioId;

            try
            {
                var current = await _ctx.Favoritos.FindAsync(empresaId, opcionId, usuarioId);

                if (current != null)
                {
                    if (current.Activo)
                    {
                        current.Activo = false;

                        _ctx.Favoritos.Update(current);
                        await _ctx.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return false;
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