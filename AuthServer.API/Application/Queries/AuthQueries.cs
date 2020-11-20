using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.Utils;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public class AuthQueries : IAuthQueries
    {
        #region Variables

        private readonly IIdentityService _identityService;
        private IOptions<AuthSettings> _settings;
        private readonly ILogger _log;

        #endregion

        #region Constructor

        public AuthQueries(IIdentityService identityService, IOptions<AuthSettings> settings, ILogger log)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        #endregion

        #region Metodos

        public async Task<dynamic> GetDatosSesionAsync()
        {
            var empresaId = _identityService.EmpresaId;
            var usuarioId = _identityService.UsuarioId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT	UsuarioId, NombreUsuario, Nombre, Imagen
                    FROM [seguridad].Usuario (NOLOCK)
                    WHERE UsuarioId = @usuarioId;

                    SELECT  EmpresaId, Codigo, Ruc, RazonSocial,
                            NombreComercial, Logo = ImagenUri, ExoneradoIGV, IncluyeIGV
                    FROM [seguridad].Empresa (NOLOCK)
                    WHERE EmpresaId = @empresaId";
                
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    await connection.OpenAsync();

                    var multiple = await connection.QueryMultipleAsync(sql, new { usuarioId, empresaId });
                    var user = multiple.Read<dynamic>().First();
                    var empresa = multiple.Read<dynamic>().First();

                    return new {
                        usuario = new {
                            user.UsuarioId,
                            user.NombreUsuario,
                            user.Nombre,
                            Avatar = (user.Imagen == null ? null : Convert.ToBase64String(user.Imagen))
                        },
                        empresa = new {
                            empresa.EmpresaId,
                            empresa.Codigo,
                            empresa.Ruc,
                            empresa.RazonSocial,
                            empresa.NombreComercial,
                            empresa.Logo,
                            empresa.ExoneradoIGV,
                            empresa.IncluyeIGV
                        }
                    };
                }
                
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException("Error al obtener información", result.EventLogId);
            }
        }

        public async Task<dynamic> ObtenerUsuariosPorRol(int empresaId, string rol, string usuario)
        {
            try
            {
                string sql = @"SELECT	u.UsuarioId, u.Correo, u.NombreUsuario, u.Nombre, u.EspacioTrabajoId
                    FROM seguridad.Usuario u (NOLOCK)
                    INNER JOIN seguridad.Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.EmpresaId = @empresaId and e.Anulado = 0
                    INNER JOIN seguridad.UsuarioRol ur (NOLOCK) ON u.UsuarioId = ur.UsuarioId
                    INNER JOIN seguridad.Rol r (NOLOCK) ON ur.RolId = r.RolId and r.NombreNormalizado = @rol
                    WHERE u.Activo = 1";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryAsync(sql, new { empresaId, rol });

                    var data = (from q in ((IEnumerable<dynamic>)query)
                        select new { 
                            q.UsuarioId,
                            q.Correo,
                            q.NombreUsuario,
                            q.Nombre,
                            q.EspacioTrabajoId
                        }).Distinct().ToList<dynamic>();

                    return data;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> ObtenerModulosAsync(string usuarioId, string aplicacionClienteId, string usuario)
        {
            try
            {
                string sql = @"SELECT	m.ModuloId, m.Nombre, m.Imagen, m.NombreRuta,
                            m.UriRuta, m.NombreCorto
                    FROM [seguridad].Modulo m
                    INNER JOIN [seguridad].DetalleEspacioTrabajo de ON m.ModuloId = de.ModuloId and de.Activo = 1
                    INNER JOIN [seguridad].Usuario u ON de.EspacioTrabajoId = u.EspacioTrabajoId and u.Activo = 1
                    WHERE u.UsuarioId = @usuarioId and m.AplicacionClienteId = @aplicacionClienteId";
                    
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryAsync(sql, new { usuarioId, aplicacionClienteId });

                    var data = (from q in ((IEnumerable<dynamic>)query)
                        select new { 
                            q.ModuloId,
                            q.Nombre,
                            q.Imagen,
                            q.NombreRuta,
                            q.UriRuta,
                            q.NombreCorto
                        }).Distinct().ToList<dynamic>();

                    return data;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> ObtenerRecursosAsync(string usuarioId, string usuario)
        {
            try
            {
                string sql = @"SELECT	r.RecursoId, r.Nombre, r.NombreHost, r.UriHost,
                            r.Activo, r.FechaRegistro
                    FROM [seguridad].Recurso r
                    INNER JOIN [seguridad].ModuloRecurso mr ON r.RecursoId = mr.RecursoId and mr.Activo = 1
                    INNER JOIN [seguridad].DetalleEspacioTrabajo de ON mr.ModuloId = de.ModuloId and de.Activo = 1
                    INNER JOIN [seguridad].Usuario u ON de.EspacioTrabajoId = u.EspacioTrabajoId and u.Activo = 1
                    WHERE u.UsuarioId = @usuarioId";
                    
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryAsync(sql, new { usuarioId });

                    var data = (from q in ((IEnumerable<dynamic>)query)
                        select new { 
                            q.RecursoId,
                            q.Nombre,
                            q.NombreHost,
                            q.UriHost,
                            q.Activo,
                            q.FechaRegistro
                        }).Distinct().ToList<dynamic>();

                    return data;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> TieneAccesoAplicacionAsync(string usuarioId, int moduloId, string usuario)
        {
            try
            {
                string sql = @"SELECT	m.ModuloId, m.Codigo, m.Nombre, m.NombreCorto,
                            m.NombreRuta, m.UriRuta, m.Icono, m.FechaRegistro, m.Activo
                    FROM [seguridad].Modulo m
                    INNER JOIN [seguridad].DetalleEspacioTrabajo de ON m.ModuloId = de.ModuloId and de.Activo = 1
                    INNER JOIN [seguridad].Usuario u ON de.EspacioTrabajoId = u.EspacioTrabajoId and u.Activo = 1
                    WHERE m.Activo = 1 and u.UsuarioId = @usuarioId and m.ModuloId = @moduloId";
                    
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { usuarioId, moduloId });

                    return query;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> ObtenerOpcionAsync(string viewType, string usuario)
        {
            var empresaId = _identityService.EmpresaId;
            var usuarioId = _identityService.UsuarioId;
            var aplicacionClienteId = _identityService.AplicacionClienteId;

            try
            {
                string sql = @"SELECT	o.OpcionId, o.Nombre, o.ViewClass, o.ViewType,
                            o.Icono, o.Activo, EsFavorito = ISNULL(f.Activo, 0)
                    FROM [seguridad].Opcion o
					INNER JOIN [seguridad].Modulo m ON o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId and m.Activo = 1
					LEFT JOIN seguridad.Favorito f ON o.OpcionId = f.OpcionId and f.EmpresaId = @empresaId and f.UsuarioId = @usuarioId
					WHERE o.ViewType = @viewType";
                    
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { empresaId, usuarioId, aplicacionClienteId, viewType });

                    return query;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<IEnumerable<OpcionDTO>> ObtenerOpcionPorModuloAsync(int moduloId, string usuario)
        {
            try
            {
                string sql = @"SELECT	o.OpcionId, o.ModuloId, o.PadreId, o.Nombre, 
                            o.Tooltip, o.Secuencia, o.ViewClass, o.ViewType,
                            o.Icono, o.Formulario, o.Activo, o.FechaRegistro
                    FROM [seguridad].Opcion o
                    WHERE o.ModuloId = @moduloId and o.Activo = 1
                    ORDER BY o.Secuencia";
                    
                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var query = await connection.QueryAsync<OpcionDTO>(sql, new { moduloId });

                    return query;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        #endregion
    }
}