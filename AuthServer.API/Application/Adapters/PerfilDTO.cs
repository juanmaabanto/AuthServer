using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class PerfilDTO
    {
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public byte[] Imagen { get; set; }
        public List<EmpresaListDTO> empresas { get; set; }
        public List<RolDTO> roles { get; set; }
    }
}