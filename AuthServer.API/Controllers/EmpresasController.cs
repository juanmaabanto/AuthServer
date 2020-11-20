using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using Expertec.Sigeco.AuthServer.API.Application.Queries;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class EmpresasController : Controller
    {
        #region "Variables"
        private readonly IEmpresaQueries _queries;

        #endregion

        #region "Constructor"

        public EmpresasController(IEmpresaQueries queries)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        #endregion

        #region  Gets

        /// <summary>
        /// Devuelve lista de empresas de la sesión activa.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("sesion/listar")]
        [ProducesResponseType(typeof(IEnumerable<EmpresaListDTO>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> ListarPorSesion()
        {
            try
            {
                var result = await _queries.ListarPorSesionAsync();

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion
    }
    
}