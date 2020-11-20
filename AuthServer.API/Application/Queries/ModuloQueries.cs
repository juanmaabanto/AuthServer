using System;
using System.Collections.Generic;
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
    public class ModuloQueries : IModuloQueries
    {
        #region Variables

        private readonly IIdentityService _identityService;
        private IOptions<AuthSettings> _settings;
        private readonly ILogger _log;

        #endregion

        #region Constructor

        public ModuloQueries(IIdentityService identityService, IOptions<AuthSettings> settings, ILogger logger)
        {
            _identityService = identityService;
            _settings = settings;
            _log = logger;
        }

        #endregion

        #region Metodos

        public async Task<dynamic> ListarAsync()
        {
            var usuarioId = _identityService.UsuarioId;
            var usuario = _identityService.Usuario;
            var aplicacionClienteId = _identityService.AplicacionClienteId;

            try
            {
                string sql = @"SELECT	m.ModuloId, m.Nombre, m.NombreCorto, m.NombreRuta, m.UriRuta,
                            m.Imagen
                    FROM seguridad.Modulo m
                    INNER JOIN seguridad.DetalleEspacioTrabajo de ON m.ModuloId = de.ModuloId and de.Activo = 1
                    INNER JOIN seguridad.Usuario us ON de.EspacioTrabajoId = us.EspacioTrabajoId and us.Activo = 1
                    WHERE m.Activo = 1 and m.AplicacionClienteId = @aplicacionClienteId and us.UsuarioId = @usuarioId;
                    
                    SELECT	m.ModuloId, o.OpcionId, m.Host, m.NombreHost, 
                            o.ViewType, Ruta = ISNULL(o.Ruta, '')
                    FROM [seguridad].Opcion o
                    INNER JOIN [seguridad].Modulo m ON o.ModuloId = m.ModuloId and m.Activo = 1 and m.AplicacionClienteId = @aplicacionClienteId
                    WHERE o.Activo = 1 and o.Reactivo = 1
                        and m.Host <> '' and m.Host <> '/';
                        
                    SELECT	r.RecursoId, r.Nombre, r.NombreHost, r.UriHost
                    FROM seguridad.Recurso r
					INNER JOIN seguridad.ModuloRecurso mr ON r.RecursoId = mr.RecursoId and mr.Activo = 1
					INNER JOIN seguridad.Modulo m ON mr.ModuloId = m.ModuloId and m.Activo = 1 and m.AplicacionClienteId = @aplicacionClienteId
                    WHERE r.Activo = 1";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    await connection.OpenAsync();

                    var multiple = await connection.QueryMultipleAsync(sql, new { aplicacionClienteId, usuarioId });
                    var modulos = multiple.Read<ModuloListDTO>();
                    var rutas = multiple.Read<RutasRemotasDTO>();
                    var recursos = multiple.Read<RecursoListDTO>();

                    return new { modulos, rutas, recursos };
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<IEnumerable<OpcionTreeListDTO>> ListarOpcionUsuarioAsync(int moduloId)
        {
            var aplicacionClienteId = _identityService.AplicacionClienteId;
            var usuarioId = _identityService.UsuarioId;
            var usuario = _identityService.Usuario;
            var rol = _identityService.Rol;
            var empresaId = _identityService.EmpresaId;

            try
            {
                if(rol == "Administrador")
                {
                    string sql = @"SELECT	o.OpcionId, o.ModuloId, o.PadreId, o.Nombre, 
                            o.Tooltip, o.Secuencia, o.ViewClass, o.ViewType,
                            o.Icono, o.Formulario, o.Activo, o.FechaRegistro,
                            o.Reactivo, m.NombreHost, m.Host, o.Ruta
                    FROM [seguridad].Opcion o
                    INNER JOIN [seguridad].Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
                    WHERE o.ModuloId = @moduloId and o.Activo = 1

					UNION ALL

					SELECT	OpcionId = -1, ModuloId = 0, PadreId = null, Nombre = 'Favoritos',
							Tooltip = 'Favoritos', Secuencia = 0, ViewClass = null, ViewType = null,
							Icono = 'x-fa fa-star', Formulario = 0, Activo = 1, FechaRegistro = CAST(GETDATE() as date),
							Reactivo = 0, NombreHost = '', Host = '', Ruta = null

					UNION ALL

					SELECT	o.OpcionId, o.ModuloId, PadreId = -1, o.Nombre, 
                            o.Tooltip, Secuencia = ROW_NUMBER() OVER (order by o.Nombre), o.ViewClass, o.ViewType,
                            o.Icono, o.Formulario, o.Activo, o.FechaRegistro,
                            o.Reactivo, m.NombreHost, m.Host, o.Ruta
					FROM seguridad.Favorito f
					INNER JOIN seguridad.Opcion o ON f.OpcionId = o.OpcionId and o.Activo = 1 and o.Formulario = 1
					INNER JOIN [seguridad].Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
					WHERE f.Activo = 1 and f.EmpresaId = @empresaId and f.UsuarioId = @usuarioId and o.ModuloId <> @moduloId";

                    using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                    {
                        connection.Open();

                        return await connection.QueryAsync<OpcionTreeListDTO>(sql, new { aplicacionClienteId, empresaId, moduloId, usuarioId });
                    }
                }
                else
                {
                    string sql = @"SELECT	o.OpcionId, o.ModuloId, o.PadreId, o.Nombre, 
                                o.Tooltip, o.Secuencia, o.ViewClass, o.ViewType,
                                o.Icono, o.Formulario, o.Activo, o.FechaRegistro,
                                o.Reactivo, m.NombreHost, m.Host, o.Ruta
                        FROM [seguridad].Opcion o
                        INNER JOIN [seguridad].Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
                        LEFT JOIN [seguridad].Autorizacion a ON o.OpcionId = a.OpcionId and a.EmpresaId = @empresaId
                            and a.UsuarioId = @usuarioId and CAST(GETDATE() as date) between a.FechaInicio and a.FechaFin
                        WHERE o.ModuloId = @moduloId and o.Activo = 1
                            and (o.Formulario = 0 or a.OpcionId is not null)
                        
                        UNION ALL

                        SELECT	OpcionId = -1, ModuloId = 0, PadreId = null, Nombre = 'Favoritos',
                                Tooltip = 'Favoritos', Secuencia = 0, ViewClass = null, ViewType = null,
                                Icono = 'x-fa fa-star', Formulario = 0, Activo = 1, FechaRegistro = CAST(GETDATE() as date),
                                Reactivo = 0, NombreHost = '', Host = '', Ruta = null

                        UNION ALL

                        SELECT	o.OpcionId, o.ModuloId, PadreId = -1, o.Nombre, 
                                o.Tooltip, Secuencia = ROW_NUMBER() OVER (order by o.Nombre), o.ViewClass, o.ViewType,
                                o.Icono, o.Formulario, o.Activo, o.FechaRegistro,
                                o.Reactivo, m.NombreHost, m.Host, o.Ruta
                        FROM seguridad.Favorito f
                        INNER JOIN seguridad.Opcion o ON f.OpcionId = o.OpcionId and o.Activo = 1 and o.Formulario = 1
                        INNER JOIN seguridad.Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
                        WHERE f.Activo = 1 and f.EmpresaId = @empresaId and f.UsuarioId = @usuarioId and o.ModuloId <> @moduloId";

                    using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                    {
                        connection.Open();

                        return await connection.QueryAsync<OpcionTreeListDTO>(sql, new { aplicacionClienteId, empresaId, moduloId, usuarioId });
                    }
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<IEnumerable<AutorizacionTreeListDTO>> ListarAutorizacionUsuarioAsync(int empresaId, string usuarioId, int moduloId)
        {
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT	EmpresaId = @empresaId, UsuarioId = @usuarioId, o.OpcionId, o.PadreId,
                            o.Nombre, o.Icono, o.Formulario, aut.FechaInicio,
                            aut.FechaFin
                    FROM seguridad.Opcion o (NOLOCK)
                    LEFT JOIN seguridad.Autorizacion aut (NOLOCK) ON o.OpcionId = aut.OpcionId and aut.EmpresaId = @empresaId and aut.UsuarioId = @usuarioId
                    WHERE o.Activo = 1 and o.ModuloId = @moduloId

                    SELECT	EmpresaId = @empresaId, UsuarioId = @usuarioId, o.OpcionId, do.DetalleOpcionId,
                            do.Nombre, daut.FechaInicio, daut.FechaFin
                    FROM seguridad.DetalleOpcion do (NOLOCK)
                    INNER JOIN seguridad.Opcion o (NOLOCK) ON do.OpcionId = o.OpcionId and o.ModuloId = @moduloId
                    LEFT JOIN seguridad.DetalleAutorizacion daut (NOLOCK) ON do.DetalleOpcionId = daut.DetalleOpcionId and do.OpcionId = daut.OpcionId 
                        and daut.EmpresaId = @empresaId and daut.UsuarioId = @usuarioId
                    WHERE do.Activo = 1 ";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    using(var multi = await connection.QueryMultipleAsync(sql, new {empresaId, usuarioId, moduloId}))
                    {
                        var data = multi.Read<AutorizacionTreeListDTO>().AsList();
                        var detalle = multi.Read<DetalleAutorizacionTreeListDTO>().AsList();

                        foreach(var d in data)
                        {
                            d.Detalles = detalle.FindAll(p => p.OpcionId == d.OpcionId);
                        }

                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> VerificarAccesoAsync(string viewType)
        {
            var aplicacionClienteId = _identityService.AplicacionClienteId;
            var empresaId = _identityService.EmpresaId;
            var rol = _identityService.Rol;
            var usuario = _identityService.Usuario;
            var usuarioId = _identityService.UsuarioId;

            try
            {
                string sql = @"SELECT   o.OpcionId, o.Nombre, o.Icono, EsFavorito = ISNULL(f.Activo, 0),
                            Autorizado = CAST(CASE WHEN aut.OpcionId is null or CAST(GETDATE() as date) not between aut.FechaInicio and aut.FechaFin THEN 0 else 1 END as bit)
                    FROM seguridad.Opcion o
					INNER JOIN seguridad.Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
					LEFT JOIN seguridad.Favorito f ON o.OpcionId = f.OpcionId and f.Activo = 1 and f.EmpresaId = @empresaId and f.UsuarioId = @usuarioId
                    LEFT JOIN seguridad.Autorizacion aut ON o.OpcionId = aut.OpcionId and aut.EmpresaId = @empresaId and aut.UsuarioId = @usuarioId 
                    WHERE o.ViewType = @viewType;
                    
                    SELECT do.Nombre
                    FROM seguridad.DetalleOpcion do
                    INNER JOIN seguridad.Opcion o ON do.OpcionId = o.OpcionId
					INNER JOIN seguridad.Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
                    INNER JOIN seguridad.DetalleAutorizacion dauth ON do.DetalleOpcionId = dauth.DetalleOpcionId and o.OpcionId = dauth.OpcionId
                        and dauth.EmpresaId = @empresaId and dauth.UsuarioId = @usuarioId and GETDATE() between dauth.FechaInicio and dauth.FechaFin
                    WHERE o.ViewType = @viewType";

                if(rol == "Administrador")
                {
                    sql = @"SELECT	o.OpcionId, o.Nombre, o.Icono, EsFavorito = ISNULL(f.Activo, 0),
								Autorizado = CAST(1 as bit)
                        FROM seguridad.Opcion o
						INNER JOIN seguridad.Modulo m ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
						LEFT JOIN seguridad.Favorito f ON o.OpcionId = f.OpcionId and f.Activo = 1 and f.EmpresaId = @empresaId and f.UsuarioId = @usuarioId
                        WHERE o.ViewType = @viewType;
                        
                        SELECT	do.Nombre
	                    FROM seguridad.DetalleOpcion do
                        INNER JOIN seguridad.Opcion o (NOLOCK) ON do.OpcionId = o.OpcionId
						INNER JOIN seguridad.Modulo m (NOLOCK) ON m.Activo = 1 and o.ModuloId = m.ModuloId and m.AplicacionClienteId = @aplicacionClienteId
                        WHERE o.ViewType = @viewType";
                }

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    using(var multi = await connection.QueryMultipleAsync(sql, new {aplicacionClienteId, empresaId, usuarioId, viewType}))
                    {
                        var data = await multi.ReadFirstOrDefaultAsync();
                        var accesos = await multi.ReadAsync();

                        if(data == null)
                        {
                            return new { status = 400 };
                        }
                        else
                        {
                            if(data.Autorizado)
                            {
                                return new { status = 200, option = new { optionId = data.OpcionId, name = data.Nombre, icon = data.Icono, access = accesos, favorite = data.EsFavorito } };
                            }
                            else
                            {
                                return new { status = 401 };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgGetError, result.EventLogId);
            }
        }

        public async Task<dynamic> AccesoAccionAsync(int opcionId)
        {
            var empresaId = _identityService.EmpresaId;
            var rol = _identityService.Rol;
            var usuario = _identityService.Usuario;
            var usuarioId = _identityService.UsuarioId;

            try
            {
                string sql = @"SELECT	name = do.NombreAccion
                    FROM seguridad.DetalleOpcion do
                    INNER JOIN seguridad.DetalleAutorizacion da ON do.DetalleOpcionId = da.DetalleOpcionId and da.EmpresaId = @empresaId
                        and da.UsuarioId = @usuarioId and GETDATE() between da.FechaInicio and da.FechaFin
                    WHERE do.OpcionId = @opcionId and Activo = 1";

                if(rol == "Administrador")
                {
                    sql = @"SELECT	name = NombreAccion
                        FROM seguridad.DetalleOpcion
                        WHERE OpcionId = @opcionId and Activo = 1";
                }

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var data = await connection.QueryAsync(sql, new { empresaId, usuarioId, opcionId });

                    return data;
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