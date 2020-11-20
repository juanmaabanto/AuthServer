using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Queries;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Expertec.Sigeco.AuthServer.API.ViewModel;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class AccountController : Controller
    {
        #region "Variables"

        private readonly IAuthQueries _queries;
        private readonly IAuthRepository _repository;
        private readonly ILogger _log;

        #endregion

        #region "Constructor"

        public AccountController(IAuthQueries queries, IAuthRepository repository, ILogger log)
        {
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        #endregion

        #region "Gets"

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("datos")]
        public async Task<IActionResult> ObtenerDatos()
        {
            try
            {
                var result = await _queries.GetDatosSesionAsync();

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion

        /// <summary>
        /// Devuelve lista de cuentas para seleccionar.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Cuentas()
        {
            var cuentas = new List<dynamic>();
            var i = 0;

            foreach(var identity in User.Identities)
            {
                if(identity.Name != null)
                {
                    var usuario = await _repository.GetUsuarioAsync(identity.Name);

                    cuentas.Add(new {
                        Autenticado = identity.IsAuthenticated,
                        Avatar = usuario.Avatar,
                        Nombre = usuario.Nombre,
                        NombreUsuario = usuario.NombreUsuario,
                        Authuser = i
                    });
                    i++;
                }
            }

            return Ok(cuentas);
        }

        /// <summary>
        /// Válida información del usuario para el login.
        /// </summary>
        /// <param name="nombreUsuario">Usuario.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("validausuario/{nombreUsuario}")]
        public async Task<IActionResult> ValidaUsuario(string nombreUsuario)
        {
            try
            {
                var usuario = await _repository.GetUsuarioAsync(nombreUsuario);

                return Ok(usuario);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var username = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;

            var user = await _repository.GetUsuarioAsync(username);

            if (user == null)
            {
                return BadRequest("Ocurrio un error al obtener los datos del usuario.");
            }

            var list = new { Name = user.Nombre, LastName = string.Empty, UserName = user.NombreUsuario, Domain = string.Empty, UserId = user.UsuarioId };

            return Ok(list);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetDetailWorkSpace")]
        public async Task<IActionResult> GetDetailWorkSpace()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;
            var aplicacionClienteId = ((ClaimsIdentity) User.Identity).FindFirst("ClienteId").Value;

            var details = await _queries.ObtenerModulosAsync(usuarioId, aplicacionClienteId, usuario);

            return Ok(details);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetResources")]
        public async Task<IActionResult> GetResources()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;

            var resources = await _queries.ObtenerRecursosAsync(usuarioId, usuario);

            return Ok(resources);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CompanyData")]
        public async Task<IActionResult> CompanyData()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;
            var empresaId = Convert.ToInt32(((ClaimsIdentity) User.Identity).FindFirst("EmpresaId").Value);

            var empresa = await _repository.CompanyData(empresaId, usuarioId, usuario);

            if (empresa == null)
            {
                return BadRequest("Ha Ocurrido un error.");
            }

            var data =  new
            {
                CompanyId = empresa.EmpresaId,
                Code = empresa.Empresa.Codigo,
                BusinessName = empresa.Empresa.RazonSocial,
                ImageUri = string.Empty,
                ExoneradoIGV = empresa.Empresa.ExoneradoIGV,
                IncluyeIGV = empresa.Empresa.IncluyeIGV
            };

            return Ok(data);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetCompanies")]
        public IActionResult GetCompanies()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;

            var company = _repository.GetCompanies(usuarioId, usuario);

            if (company == null)
            {
                return BadRequest("Ha Ocurrido un error.");
            }

            var data = (from c in company
                select new {
                    Code = c.Codigo,
                    BusinessName = c.RazonSocial
                });

            return Ok(data);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("WorkSpace/GetCompanies")]
        public IActionResult GetCompaniesByWorkSpace()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;

            var companies = _repository.GetCompanies(usuarioId, usuario);

            return Ok(companies);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("getcompanybyuser")]
        public IActionResult GetCompanyByUser(string usuarioId)
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var userId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;


            var company = _repository.GetCompanies(usuarioId is null ? userId : usuarioId, usuario);

            if (company == null)
            {
                return BadRequest("Ha Ocurrido un error.");
            }

            var data = (from c in company
                select new {
                    EmpresaId = c.EmpresaId,
                    Codigo = c.Codigo,
                    Nombre = c.RazonSocial
                });

            return Ok(data);
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetApplicationAccess")]
        public async Task<IActionResult> GetApplicationAccess(int moduloId)
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;

            var modulo = await _queries.TieneAccesoAplicacionAsync(usuarioId, moduloId, usuarioId);

            if (modulo == null)
            {
                return BadRequest("No tiene acceso al módulo o no existe.");
            }

            if (!modulo.Activo)
            {
                return BadRequest("El módulo no se encuentra activo.");
            }

            var data = new
            {
                ApplicationId = modulo.ModuloId,
                Name = modulo.Nombre,
                ShortName = modulo.NombreCorto,
                Icon = modulo.Icono
            };

            return Ok(data);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetOptionAccess")]
        public async Task<IActionResult> GetOptionAccess(string viewType)
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;

            var result = await _queries.ObtenerOpcionAsync(viewType, usuario);

            if (result == null)
            {
                return BadRequest("La opción solicitada no existe.");
            }

            if (!result.Activo)
            {
                return BadRequest("La opción no se encuentra activa.");
            }

            var data = new
            {
                favorite = result.EsFavorito,
                OptionId = result.OpcionId,
                Name = result.Nombre,
                ViewClass = result.ViewClass,
                ViewType = result.ViewType,
                Icon = result.Icono
            };

            return Ok(data);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetOnlyRoleUser")]
        public async Task<IActionResult> GetOnlyRoleUser()
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;
            var empresaId = Convert.ToInt32(((ClaimsIdentity) User.Identity).FindFirst("EmpresaId").Value);

            var result = await _queries.ObtenerUsuariosPorRol(empresaId, "usuario", usuario);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("IsAuthorized")]
        public IActionResult IsAuthorized(string controllerName, string actionName)
        {
            var usuarioId = ((ClaimsIdentity) User.Identity).FindFirst("UsuarioId").Value;
            var empresaId = Convert.ToInt32(((ClaimsIdentity) User.Identity).FindFirst("EmpresaId").Value);
            var rol = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Role).Value;
            var aplicacionClienteId = ((ClaimsIdentity) User.Identity).FindFirst("ClienteId").Value;

            if (rol.Equals("Administrador"))
            {
                return Ok(true);
            }
            else
            {
                if (_repository.IsAuthorize(usuarioId, empresaId, controllerName, actionName, aplicacionClienteId))
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest("No tiene autorización.");
                }
            }
            
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SaveAuthorization")]
        public async Task<IActionResult> SaveAuthorization([FromBody]object item, string userId, byte companyId)
        {
            var usuario = ((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.Name).Value;

            List<Dictionary<string, object>> lst = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(item.ToString());

            int result = await _repository.SaveAuthorizationAsync(lst, userId, companyId, usuario);

            if (result == 0)
            {
                return BadRequest("Ocurrio un error al registrar.");
            }

            return Ok(result);
        }
    }
}