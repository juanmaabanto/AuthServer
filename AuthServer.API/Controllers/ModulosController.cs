using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using Expertec.Sigeco.AuthServer.API.Application.Queries;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Expertec.Sigeco.AuthServer.API.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class ModulosController : Controller
    {
        #region "Variables"
        private readonly IModuloQueries _queries;
        private readonly IModuloRepository _repository;

        #endregion

        #region "Constructor"

        public ModulosController(IModuloQueries queries, IModuloRepository repository)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _repository = repository ?? throw new ArgumentNullException(nameof(queries));
        }

        #endregion

        #region "Gets"

        /// <summary>
        /// Lista módulos del espacio de trabajo por aplicación cliente.
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<dynamic>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var result = await _queries.ListarAsync();

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        /// <summary>
        /// Lista opciones de un módulo por usuario.
        /// </summary>
        /// <param name="moduloId">Id del módulo.</param>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{moduloId:int}/opciones/arbol")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> ListarOpciones(int moduloId)
        {
            try
            {
                var opciones = await _queries.ListarOpcionUsuarioAsync(moduloId);

                var data = GetTreeList(opciones, null);

                return Ok(data);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        /// <summary>
        /// Devuelve lista de opciones con su autorización por usuario.
        /// </summary>
        /// <param name="empresaId"></param>
        /// <param name="usuarioId"></param>
        /// <param name="moduloId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{moduloId:int}/autorizaciones/usuario")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> ListarAutorizacionUsuario(int empresaId, string usuarioId, int moduloId)
        {
            try
            {
                var opciones = await _queries.ListarAutorizacionUsuarioAsync(empresaId, usuarioId, moduloId);

                var data = GetAuthorizationTreeList(opciones, null);

                return Ok(data);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("opciones/{opcionId:int}/accion/acceso")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> ObtenerAccesoAccion(int opcionId)
        {
            try
            {
                var result = await _repository.ObtenerAccesoAccionAsync(opcionId);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("opciones/acceso")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> VerificarAcceso(string viewType)
        {
            try
            {
                var result = await _queries.VerificarAccesoAsync(viewType);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("acciones/acceso")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> AccesoAccionAsync(int opcionId)
        {
            try
            {
                var result = await _queries.AccesoAccionAsync(opcionId);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion

        #region "Posts"

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("favoritos")]
        public async Task<IActionResult> AgregarFavorito(int optionId)
        {
            var result = await _repository.AgregarFavoritoAsync(optionId);

            return Ok(result);
        }

        #endregion

        #region Puts

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("favoritos")]
        public async Task<IActionResult> QuitarFavorito(int optionId)
        {
            var result = await _repository.QuitarFavoritoAsync(optionId);

            return Ok(result);
        }

        #endregion

        #region Metodos

        public List<object> GetTreeList(IEnumerable<OpcionTreeListDTO> options, int? parent)
        {
            List<object> childs = new List<object>();

            var list = from opts in options
                       where opts.PadreId == parent
                       orderby opts.Secuencia
                       select new
                       {
                           OptionId = opts.OpcionId,
                           Name = opts.Nombre,
                           Tooltip = opts.Tooltip,
                           ViewClass = opts.ViewClass,
                           ViewType = opts.ViewType,
                           IconCls = opts.Icono,
                           Leaf = opts.Formulario,
                           CreationDate = opts.FechaRegistro,
                           Reactivo = opts.Reactivo,
                           NombreHost = opts.NombreHost,
                           Host = opts.Host,
                           Ruta = opts.Ruta
                       };

            foreach (var item in list)
            {
                var m = new
                {
                    menu = item.OptionId,
                    text = item.Name,
                    qtip = item.Tooltip,
                    viewClass = item.ViewClass,
                    viewType = item.ViewType,
                    leaf = item.Leaf,
                    iconCls = item.IconCls,
                    rowCls = item.Leaf && (DateTime.Now.Date - item.CreationDate.Date).TotalDays <= 30 ? "nav-tree-badge nav-tree-badge-new ": string.Empty,
                    Reactivo = item.Reactivo,
                    NombreHost = item.NombreHost,
                    Host = item.Host,
                    Ruta = item.Ruta,
                    children = item.Leaf ? null : GetTreeList(options, item.OptionId)
                };

                if (item.Leaf || m.children.Count > 0)
                {
                    childs.Add(m);
                }
            }

            return childs;
        }

        public List<object> GetAuthorizationTreeList(IEnumerable<AutorizacionTreeListDTO> options, int? parent)
        {
            List<object> childs = new List<object>();

            var list = from auths in options
                       where auths.PadreId == parent
                       select new
                       {
                           OptionId = auths.OpcionId,
                           Name = auths.Nombre,
                           IconCls = auths.Icono,
                           Leaf = auths.Formulario,
                           StartDate = auths.FechaInicio,
                           EndDate = auths.FechaFin,
                           Allowed = auths.FechaInicio == null ? false : true,
                           SYS_DetailAuthorization = auths.Detalles
                       };

            foreach (var item in list)
            {
                var m = new
                {
                    optionId = item.OptionId,
                    detailOptionId = 0,
                    name = item.Name,
                    iconCls = item.IconCls,
                    leaf = (item.Leaf && item.SYS_DetailAuthorization.Count > 0 ? false : item.Leaf),
                    startDate = item.StartDate,
                    endDate = item.EndDate,
                    allowed = item.Leaf ? item.Allowed : new bool?(),
                    children = item.Leaf ? (item.SYS_DetailAuthorization.Count > 0 ? GetDetailAuthorizationTreeList(item.SYS_DetailAuthorization) : null) : GetAuthorizationTreeList(options, item.OptionId),
                    isEdit = item.Leaf
                };

                if (item.Leaf || m.children.Count > 0)
                {
                    childs.Add(m);
                }
            }

            return childs;
        }

        public static List<object> GetDetailAuthorizationTreeList(IEnumerable<DetalleAutorizacionTreeListDTO> details)
        {
            List<object> childs = new List<object>();

            var list = from d in details
                       select new
                       {
                           OptionId = d.OpcionId,
                           DetailOptionId = d.DetalleOpcionId,
                           Name = d.Nombre,
                           StartDate = d.FechaInicio,
                           EndDate = d.FechaFin,
                           Allowed = d.FechaInicio == null ? false : true
                       };

            foreach (var item in list)
            {
                var m = new
                {
                    optionId = item.OptionId,
                    detailOptionId = item.DetailOptionId,
                    name = item.Name,
                    iconCls = "x-fa fa-ticket",
                    leaf = true,
                    startDate = item.StartDate,
                    endDate = item.EndDate,
                    allowed = item.Allowed,
                    isEdit = true
                };

                childs.Add(m);
            }

            return childs;
        }

        #endregion

    }
    
}