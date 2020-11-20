using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Expertec.Sigeco.AuthServer.API.Providers
{
    public static class CookieAuthenticationProvider
    {
        public static Task RedirectToLoginAsync(RedirectContext<CookieAuthenticationOptions> context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public static async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context)
        {
            if(context.Request.Path.Value == "/oauth/check")
            {
                var query = HttpUtility.ParseQueryString(context.Request.QueryString.Value);
                var authuser = query["authuser"] ?? "0";
                var myIdentity = context.Principal.Identities.ElementAtOrDefault(Convert.ToInt32(authuser));

                if(myIdentity is null || !myIdentity.IsAuthenticated)
                {
                    context.RejectPrincipal();
                    return;
                }

                var sello = myIdentity.Claims.FirstOrDefault(claim => claim.Type == "SelloSeguridad")?.Value;
                var usuarioId = myIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                if(usuarioId is null)
                {
                    context.RejectPrincipal();
                    return;
                }

                var manager = context.HttpContext.RequestServices.GetRequiredService<UserManager<Usuario>>();
                var usuario = await manager.Users.FirstOrDefaultAsync(p => p.Id == usuarioId);

                if(usuario is null || sello is null || usuario.SecurityStamp != sello)
                {
                    var principal = new ClaimsPrincipal();

                    foreach (var identity in context.Principal.Identities)
                    {
                        if(identity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value == usuario.Id)
                        {
                            var claims = new List<Claim> { 
                                new Claim(ClaimTypes.Name, usuario.UserName), 
                                new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                                new Claim("SelloSeguridad", usuario.SecurityStamp)
                            };
                            
                            principal.AddIdentity(new ClaimsIdentity(claims));
                        }
                        else
                        {
                            principal.AddIdentity(identity);
                        }
                    }

                    context.ReplacePrincipal(principal);
                    context.ShouldRenew = true;
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return;
                }
            }

            
        }
    }
}