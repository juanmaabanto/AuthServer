using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Expertec.Sigeco.AuthServer.API.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        #region Variabes

        private readonly IAuthRepository _repository;
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<AuthSettings> _settings;

        #endregion

        #region Constructor

        public AuthenticationController(IAuthRepository repository, IDataProtectionProvider protectorProvider, IOptions<AuthSettings> settings, IWebHostEnvironment env)
        {
            _repository = repository;
            _protector = protectorProvider.CreateProtector("Expertec.Sigeco.AuthServer.API");
            _settings = settings;
            _env = env;
        }

        #endregion

        #region Gets

        [Authorize]
        [HttpGet("~/oauth/check")]
        public async Task<IActionResult> Check(string clientId, int authuser)
        {
            var identity = User.Identities.ElementAtOrDefault(authuser) ?? (ClaimsIdentity) User.Identity;

            if(!identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _repository.GetUsuarioAplicacionAsync(identity.Name, clientId);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("~/signin/accounts")]
        public async Task<IActionResult> Accounts()
        {
            var accounts = new List<string>();

            if(Request.Cookies["Sigeco.Accounts"] != null && Request.Cookies["Sigeco.Accounts"].Length > 0)
            {
                accounts = _protector.Unprotect(Request.Cookies["Sigeco.Accounts"]).Split(";").ToList();
            }

            var cuentas = new List<dynamic>();

            foreach(var account in accounts)
            {
                var identity = User.Identities.FirstOrDefault(p => p.Name == account);
                var usuario = await _repository.GetUsuarioAsync(account);
                var autenticado = identity == null ? false : identity.IsAuthenticated;

                cuentas.Add(new {
                    Autenticado = autenticado,
                    Avatar = usuario.Avatar,
                    Nombre = usuario.Nombre,
                    NombreUsuario = usuario.NombreUsuario,
                    Authuser = identity == null ? -1 : User.Identities.ToList().IndexOf(identity)
                });
            }

            return Ok(cuentas);
        }   

        [AllowAnonymous]
        [HttpGet("~/servicelogin")]
        public IActionResult ServiceLogin(int authuser, string @continue)
        {
            var identity = User.Identities.ElementAt(authuser) ?? (ClaimsIdentity) User.Identity;

            if (!(Uri.TryCreate(@continue, UriKind.Absolute, out Uri outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)))
            {
                @continue = _settings.Value.ReturnUrl;
            }

            var uriBuilder = new UriBuilder(@continue);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            
            query["authuser"] = User.Identities.ToList().IndexOf(identity) == -1 ? "0" : User.Identities.ToList().IndexOf(identity).ToString();
            uriBuilder.Query = query.ToString();

            return Ok(new { Url = uriBuilder.Uri.ToString() });
        }

        #endregion

        #region Posts

        [AllowAnonymous]
        [HttpPost("~/changepassword")]
        public async Task<IActionResult> ChangePassword(string username, string password, string newPassword, string confirmPassword)
        {
            try
            {   
                if(!newPassword.Equals(confirmPassword))
                {
                    throw new AuthDomainException("Las contraseñas no coinciden");
                }

                var result = await _repository.CambiarClaveAsync(username, password, newPassword);
                
                return Ok(result);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("~/signin")]
        public async Task<IActionResult> SignIn(string username, string password, bool isPersistent, string clienteId, string @continue)
        {
            try
            {
                var usuario = await _repository.AutenticarAsync(username, password, clienteId);

                if(usuario.RequiereCambioClave || (usuario.ExpiraClaveHabilitado && usuario.ExpiraClave.HasValue && usuario.ExpiraClave.Value.DateTime < DateTime.UtcNow ) ) 
                {
                    return Ok(new { requiereCambioClave = true });
                }

                var principal = new ClaimsPrincipal();
                var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name, usuario.UserName), 
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                    new Claim("SelloSeguridad", usuario.SecurityStamp)
                };
                var identity = new ClaimsIdentity(claims, "Sigeco");
                var accounts = new List<string>();

                if(Request.Cookies["Sigeco.Accounts"] != null && Request.Cookies["Sigeco.Accounts"].Length > 0)
                {
                    accounts = _protector.Unprotect(Request.Cookies["Sigeco.Accounts"]).Split(";").ToList();
                }
                if(!accounts.Contains(usuario.UserName))
                {
                    accounts.Add(usuario.UserName);
                }

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(365);
                cookieOptions.HttpOnly = true;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = _env.EnvironmentName != Environments.Development;

                Response.Cookies.Append("Sigeco.Accounts", _protector.Protect(String.Join(";", accounts)), cookieOptions);

                foreach(var i in User.Identities)
                {
                    if(i.Name != null)
                    {
                        if(i.Name == identity.Name)
                        {
                            principal.AddIdentity(identity);
                        }
                        else
                        {
                            principal.AddIdentity(i);
                        }
                    }
                }

                if(!principal.Identities.Contains(identity))
                {
                    principal.AddIdentity(identity);
                }

                if (!(Uri.TryCreate(@continue, UriKind.Absolute, out Uri outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)))
                {
                    @continue = _settings.Value.ReturnUrl;
                }

                var uriBuilder = new UriBuilder(@continue);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                
                query["authuser"] = principal.Identities.ToList().IndexOf(identity) == -1 ? "0" : principal.Identities.ToList().IndexOf(identity).ToString();
                uriBuilder.Query = query.ToString();

                if(isPersistent)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = true });
                }
                else
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                }

                return Ok(new { Url = uriBuilder.Uri.ToString() });
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch (CryptographicException)
            {
                return BadRequest(new ErrorViewModel(0, "Ocurrio un error al leer las Cookies"));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("~/signin/chooseraccount")]
        public IActionResult ChosserAccount(string username)
        {
            var identity = User.Identities.FirstOrDefault(p => p.Name == username);

            if(identity == null)
            {
                return Ok(false);
            }
            else
            {
                return Ok(new{
                    nombreUsuario = username,
                    autenticado = identity.IsAuthenticated,
                    authuser = User.Identities.ToList().IndexOf(identity)
                });
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("~/signin/removelocalaccount")]
        public ActionResult RemoveLocalAccount(string username)
        {
            try
            {
                var accounts = new List<string>();

                if(Request.Cookies["Sigeco.Accounts"] != null && Request.Cookies["Sigeco.Accounts"].Length > 0)
                {
                    accounts = _protector.Unprotect(Request.Cookies["Sigeco.Accounts"]).Split(";").ToList();
                }

                accounts.RemoveAll(p => p == username);

                if(accounts.Count == 0) {
                    Response.Cookies.Delete("Sigeco.Accounts");
                }
                else {
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(365);
                    cookieOptions.HttpOnly = true;
                    cookieOptions.SameSite = SameSiteMode.None;
                    cookieOptions.Secure = _env.EnvironmentName != Environments.Development;

                    Response.Cookies.Append("Sigeco.Accounts", _protector.Protect(String.Join(";", accounts)), cookieOptions);
                }
                return Ok(username);
            }
            catch (CryptographicException)
            {
                return BadRequest(new ErrorViewModel(0, "Ocurrio un error al leer las Cookies"));
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("~/signin/user/lookup")]
        public async Task<IActionResult> Lookup(string username)
        {
            try
            {
                var result = await _repository.GetUsuarioAsync(username);

                return Ok(result);
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
        
        [AllowAnonymous]
        [HttpGet("~/signout"), HttpPost("~/signout")]
        public ActionResult SignOut()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion
    }
}