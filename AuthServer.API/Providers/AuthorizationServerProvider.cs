using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;

namespace Expertec.Sigeco.AuthServer.API.Providers
{
    public class AuthorizationServerProvider : OpenIdConnectServerProvider
    {

        #region Variables

        private readonly IAuthRepository _repository;


        #endregion

        #region Constructor

        public AuthorizationServerProvider(IAuthRepository repository)
        {
            _repository = repository;
        }

        #endregion

        #region Overrides

        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            if (!context.Request.IsAuthorizationCodeFlow() && !context.Request.IsImplicitFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: "Este servidor de autorización solo admite el código de autorización y el flujo implícito.");

                return;
            }

            if (!string.IsNullOrEmpty(context.Request.ResponseMode) && !context.Request.IsFormPostResponseMode() &&
                                                                       !context.Request.IsFragmentResponseMode() &&
                                                                       !context.Request.IsQueryResponseMode())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "El especificado 'response_mode' no es soportado.");

                return;
            }

            var aplicacion = await _repository.GetAplicacionClienteAsync(context.ClientId, context.HttpContext.RequestAborted);

            if (aplicacion == null)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "El identificador de cliente especificado no es válido.");

                return;
            }

            if (string.IsNullOrEmpty(context.RedirectUri) || 
                !string.Equals(context.RedirectUri, aplicacion.RedirigirUri, StringComparison.Ordinal))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "El especificado 'redirect_uri' es inválido.");
                return;
            }

            context.Validate(aplicacion.RedirigirUri);
        }

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            if (!context.Request.IsAuthorizationCodeGrantType() && !context.Request.IsRefreshTokenGrantType() && !context.Request.IsPasswordGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: @"Este servidor de autorización solo acepta el código de autorización, los tipos 
                        de concesión de token de actualización y grant_type=password.");

                return;
            }

            if (string.IsNullOrEmpty(context.ClientId) || string.IsNullOrEmpty(context.ClientSecret))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "Faltan los parametros obligatorios 'client_id'/'client_secret'.");

                return;
            }

            var aplicacion = await _repository.GetAplicacionClienteAsync(context.ClientId, context.HttpContext.RequestAborted);

            if (aplicacion == null)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "El identificador de cliente especificado no es válido.");

                return;
            }

            if (!string.Equals(context.ClientSecret, aplicacion.Secreto, StringComparison.Ordinal))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Las credenciales especificadas del cliente son inválidas.");

                return;
            }

            context.Validate();
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            string allowedOrigin = "*";
            string companyCode = context.Request.GetParameter("company_code").HasValue ? (string)(context.Request.GetParameter("company_code").Value) : string.Empty;

            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            if(context.Request.IsPasswordGrantType())
            {
                try
                {
                    var user = await _repository.AutenticarAsync(context.Request.Username, context.Request.Password, string.Empty);
                    var empresaId = await (companyCode.Length == 0 ? _repository.GetEmpresaIdAsync(user.Id) : _repository.GetEmpresaIdAsync(user.Id, companyCode));
                    string rol = await _repository.GetRolAsync(user.Id);

                    var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, 
                    OpenIdConnectConstants.Claims.Name, 
                    OpenIdConnectConstants.Claims.Role);

                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[unique id]");

                    identity.AddClaim("UsuarioId", user.Id,
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken);
                    
                    identity.AddClaim(OpenIdConnectConstants.Claims.Audience, "sigecoservices",
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken);

                    identity.AddClaim(ClaimTypes.Name, user.UserName,
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken);

                    identity.AddClaim(ClaimTypes.Role, rol,
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken);

                    identity.AddClaim("EmpresaId", empresaId.ToString(),
                        OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken);

                    identity.AddClaim("ClientId", context.Request.ClientId,
                        OpenIdConnectConstants.Destinations.AccessToken,
                        OpenIdConnectConstants.Destinations.IdentityToken);

                    var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            { "client_id", context.Request.ClientId },
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
                    }.Intersect(context.Request.GetScopes()));

                    context.Validate(ticket);

                }
                catch(AuthDomainException ex)
                {
                    context.Reject("error", ex.Message);
                    return;
                }
                catch (Exception)
                {
                    context.Reject("error", "Ocurrio un error en el servidor");
                    return;
                }
            }
        }

        public override Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context)
        {
            if (context.Error == null)
            {
                foreach (var property in context.Ticket.Properties.Items)
                {
                    if(!property.Key.StartsWith("."))
                    {
                        context.Response.AddParameter(property.Key, new OpenIdConnectParameter(property.Value));
                    }
                    
                }
            }

            return Task.FromResult(0);
        }

        public override Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            if (context.Error == null)
            {
                foreach (var property in context.Ticket.Properties.Items)
                {
                    context.Response.AddParameter(property.Key, new OpenIdConnectParameter(property.Value));
                }
            }

            return Task.FromResult(0);
        }

        #endregion
        
    }
}