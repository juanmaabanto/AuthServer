using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Filters;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        #region Variables

        private readonly IAuthRepository _repository;

        #endregion

        #region Constructor

        public AuthorizationController(IAuthRepository repository) 
        {
            _repository = repository;
        }

        #endregion

        #region Posts

        [Authorize]
        [FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/authorize")]
        public async Task<IActionResult> Accept(CancellationToken cancellationToken)
        {
            var response = HttpContext.GetOpenIdConnectResponse();
            if (response != null)
            {
                return BadRequest(response);
            }

            var request = HttpContext.GetOpenIdConnectRequest();
            if (request == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.ServerError,
                    ErrorDescription = "Ha ocurrido un error interno."
                });
            }

            Int32.TryParse(request.GetParameter("authuser").ToString(), out int  authuser);
            var companyCode = request.GetParameter("company_code").ToString();
            var user = User.Identities.ElementAtOrDefault(authuser);

            if(user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.UnauthorizedClient,
                    ErrorDescription = "No se encontro sesión del usuario."
                });
            }

            if(!user.IsAuthenticated)
            {
                return Unauthorized();
            }

            try
            {
                var usuarioId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
                var empresaId = await (companyCode.Length == 0 ? _repository.GetEmpresaIdAsync(usuarioId) : _repository.GetEmpresaIdAsync(usuarioId, companyCode));
                string rol = await _repository.GetRolAsync(usuarioId);
                var usuario = await _repository.GetUsuarioPorIdAsync(usuarioId);

                var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, 
                    OpenIdConnectConstants.Claims.Name, 
                    OpenIdConnectConstants.Claims.Role);

                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[unique id]");

                identity.AddClaim("UsuarioId", usuarioId,
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken);
                
                identity.AddClaim(OpenIdConnectConstants.Claims.Audience, "sigecoservices",
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim(ClaimTypes.Name, user.FindFirst(ClaimTypes.Name).Value,
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim(ClaimTypes.Role, rol,
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim("EmpresaId", empresaId.ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                identity.AddClaim("ClienteId", request.ClientId.ToString(),
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);

                var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                { "client_id", request.ClientId },
                                { "authuser", authuser.ToString() },
                                { "company_code", companyCode },
                            });

                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    props,
                    OpenIdConnectServerDefaults.AuthenticationScheme);

                ticket.SetScopes(new[]
                {
                    /* openid: */ OpenIdConnectConstants.Scopes.OpenId,
                    /* email: */ OpenIdConnectConstants.Scopes.Email,
                    /* profile: */ OpenIdConnectConstants.Scopes.Profile
                }.Intersect(request.GetScopes()));

                //Otorgamos acceso a aplicación
                var aplicacion = await _repository.GetAplicacionClienteAsync(request.ClientId, cancellationToken);

                if(aplicacion.EsTercero)
                {
                    await _repository.AgregarUsuarioAplicacionAsync(usuarioId, request.ClientId);
                }
                //

                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = ex.Message
                });
            }
        }

        #endregion
    }
}