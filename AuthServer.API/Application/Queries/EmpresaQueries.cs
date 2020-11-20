using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Expertec.Sigeco.AuthServer.API.Application.Queries
{
    public class EmpresaQueries : IEmpresaQueries
    {
        #region Variables

        private readonly IIdentityService _identityService;
        private IOptions<AuthSettings> _settings;
        private readonly ILogger _log;

        #endregion

        #region Constructor

        public EmpresaQueries(IIdentityService identityService, IOptions<AuthSettings> settings, ILogger logger)
        {
            _identityService = identityService;
            _settings = settings;
            _log = logger;
        }

        #endregion

        #region Metodos

        public async Task<IEnumerable<EmpresaListDTO>> ListarPorSesionAsync()
        {
            var usuarioId = _identityService.UsuarioId;
            var usuario = _identityService.Usuario;

            try
            {
                string sql = @"SELECT e.EmpresaId, e.Codigo, e.Ruc, e.RazonSocial, ue.Principal
                    FROM seguridad.Empresa e (NOLOCK)
                    INNER JOIN seguridad.UsuarioEmpresa ue (NOLOCK) ON e.EmpresaId = ue.EmpresaId
                    WHERE e.Activo = 1 and e.Anulado = 0 and ue.UsuarioId = @usuarioId";

                using (var connection = new SqlConnection(_settings.Value.ConnectionStrings.Api))
                {
                    connection.Open();

                    var data = await connection.QueryAsync<EmpresaListDTO>(sql, new{ usuarioId });

                    return data;
                }
            }
            catch (Exception ex)
            {
                var result = await _log.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException("Error obteniendo empresas de usuario", result.EventLogId);
            }
        }

        #endregion
    }
}