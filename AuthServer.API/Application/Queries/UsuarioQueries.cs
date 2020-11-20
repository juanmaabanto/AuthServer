using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Utils;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public class UsuarioQueries : IUsuarioQueries
    {
        #region Variables

        private readonly IIdentityService _identityService;
        private IOptions<AuthSettings> _settings;
        private readonly ILogger _log;

        #endregion

        #region Constructor

        public UsuarioQueries(IIdentityService identityService, IOptions<AuthSettings> settings, ILogger logger)
        {
            _identityService = identityService;
            _settings = settings;
            _log = logger;
        }

        #endregion

        #region Metodos

        public async Task<(int total, IEnumerable<UsuarioListDTO> data)> ListarAsync(string nombre, string sort, int pageSize, int start,
            CancellationToken cancellationToken)
        {
            var orderby = Helper.GetOrderByFormat(sort);
            var empresaId = _identityService.EmpresaId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT COUNT(*)
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    WHERE e.EmpresaId = @empresaId 
						and u.Nombre + ' ' + u.NombreUsuario like '%' + ISNULL(@nombre, '') + '%';
                    
                    SELECT	u.UsuarioId, u.Nombre, u.NombreUsuario, u.Correo,
                            u.Telefono, u.Activo
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    WHERE e.EmpresaId = @empresaId
						and u.Nombre + ' ' + u.NombreUsuario like '%' + ISNULL(@nombre, '') + '%'
                    ORDER BY "+ (string.IsNullOrEmpty(orderby) ? "Nombre" : orderby) +
                    " OFFSET @start ROWS FETCH NEXT @pageSize ROWS ONLY";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    await connection.OpenAsync(cancellationToken);

                    var command = new CommandDefinition(sql, new { empresaId, nombre, start, pageSize },
                        commandType: CommandType.Text, cancellationToken: cancellationToken
                    );

                    using(var multi = await connection.QueryMultipleAsync(command))
                    {
                        var total = multi.Read<int>().First();
                        var data = multi.Read<UsuarioListDTO>().AsList();

                        return (total, data);
                    }
                }
            }
            catch(OperationCanceledException)
            {
                throw new AuthDomainException("Se cancelo la operación");
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<UsuarioDTO> ObtenerAsync(string usuarioId)
        {
            var empresaId = _identityService.EmpresaId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT	u.UsuarioId, u.Nombre, u.NombreUsuario, u.Correo, 
                            u.Telefono, u.BloqueoHabilitado, u.ExpiraClaveHabilitado, u.Activo,
							p.NroDocumento, p.PrimerNombre, p.SegundoNombre, p.ApellidoPaterno,
							p.ApellidoMaterno
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
					INNER JOIN [seguridad].Persona p (NOLOCK) ON u.PersonaId = p.PersonaId
                    WHERE e.EmpresaId = @empresaId
						and u.UsuarioId = @usuarioId;
                        
                    SELECT	ue.EmpresaId, e.RazonSocial, ue.Principal
                    FROM seguridad.UsuarioEmpresa ue
                    INNER JOIN seguridad.Empresa e ON ue.EmpresaId = e.EmpresaId and e.Activo = 1 and e.Anulado = 0
                    WHERE ue.UsuarioId = @usuarioId";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    using(var multi = await connection.QueryMultipleAsync(sql, new { empresaId, usuarioId }))
                    {
                        var data = multi.Read<UsuarioDTO>().FirstOrDefault();
                        var empresas = multi.Read<EmpresaListDTO>().AsList();

                        if(data == null)
                        {
                            throw new KeyNotFoundException();
                        }

                        data.Empresas = empresas;

                        return data;
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<byte[]> ObtenerBytesAsync(string nombre)
        {
            var empresaId = _identityService.EmpresaId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT	u.UsuarioId, u.Nombre, u.NombreUsuario, u.Correo,
                            u.Telefono, u.Activo
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    WHERE e.EmpresaId = @empresaId
						and u.Nombre + ' ' + u.NombreUsuario like '%' + ISNULL(@nombre, '') + '%'
                    ORDER BY Nombre";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var data = await connection.QueryAsync<UsuarioListDTO>(sql, new{ empresaId, nombre });

                    return Helper.GetBytesCSV(data);
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<PerfilDTO> ObtenerPerfilAsync()
        {
            var usuarioId = _identityService.UsuarioId;

            try
            {
                return await ObtenerPerfilAsync(usuarioId);
            }
            catch (AuthDomainException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
        }

        public async Task<PerfilDTO> ObtenerPerfilAsync(string usuarioId)
        {
            var empresaId = _identityService.EmpresaId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT	u.UsuarioId, u.Nombre, u.NombreUsuario, u.Correo, 
                            Telefono = ISNULL(u.Telefono, ''), u.Imagen
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    WHERE e.EmpresaId = @empresaId 
						and u.UsuarioId = @usuarioId;

                    SELECT	e.EmpresaId, e.Ruc, e.RazonSocial 
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    WHERE e.EmpresaId = @empresaId 
                        and u.UsuarioId = @usuarioId;

                    SELECT	r.Nombre
                    FROM [seguridad].Usuario u (NOLOCK)
                    INNER JOIN [seguridad].Empresa e (NOLOCK) ON u.EspacioTrabajoId = e.EspacioTrabajoId and e.Anulado = 0
                    INNER JOIN [seguridad].UsuarioRol ur (NOLOCK) ON u.UsuarioId = ur.UsuarioId
                    INNER JOIN [seguridad].Rol r (NOLOCK) ON ur.RolId = r.RolId
                    WHERE e.EmpresaId = @empresaId 
                        and u.UsuarioId = @usuarioId";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    using(var multi = await connection.QueryMultipleAsync(sql, new {empresaId, usuarioId}))
                    {
                        var data = multi.Read<PerfilDTO>().FirstOrDefault();
                        var empresas = multi.Read<EmpresaListDTO>().AsList();
                        var roles = multi.Read<RolDTO>().AsList();

                        if(data == null)
                        {
                            throw new KeyNotFoundException();
                        }
                        
                        data.empresas = empresas;
                        data.roles = roles;

                        return data;
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
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