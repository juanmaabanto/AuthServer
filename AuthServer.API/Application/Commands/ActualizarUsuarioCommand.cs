using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using MediatR;

namespace Expertec.Sigeco.AuthServer.API.Application.Commands
{
    public class ActualizarUsuarioCommand : IRequest<string>
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Clave { get; set; }

        public bool BloqueoHabilitado { get; set; }

        public bool ExpiraClaveHabilitado { get; set; }

        public bool Activo { get; set; }

        public bool ReiniciarClave { get; set; }
        public string NroDocumento { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string PrimerNombre {get; set;}
        public string SegundoNombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }

        public IEnumerable<EmpresaListDTO> Empresas { get; set; }
    }
}