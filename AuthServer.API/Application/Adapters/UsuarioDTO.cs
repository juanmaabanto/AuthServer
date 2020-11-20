using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class UsuarioDTO
    {
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool BloqueoHabilitado { get; set; }
        public bool ExpiraClaveHabilitado { get; set; }
        public bool Activo { get; set; }
        public string NroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public IEnumerable<EmpresaListDTO> Empresas { get; set; }
    }
}