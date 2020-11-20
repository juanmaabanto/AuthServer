using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Expertec.Sigeco.AuthServer.API.Application.Services
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context; 

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int EmpresaId => Convert.ToInt32(_context.HttpContext.User.FindFirst("EmpresaId").Value);

        public string Rol => _context.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        public string Usuario => _context.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        public string UsuarioId => _context.HttpContext.User.FindFirst("UsuarioId").Value;

        public string AplicacionClienteId => _context.HttpContext.User.FindFirst("ClienteId").Value;

        public string Token => _context.HttpContext.Request.Headers[HeaderNames.Authorization];
  
    }
}