using Microsoft.AspNetCore.Identity;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class UsuarioRol : IdentityUserRole<string>
    {
        public IdentityRole Rol { get; private set; }
        
    }
}