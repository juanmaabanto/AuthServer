using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Expertec.Sigeco.AuthServer.API.Application.Commands
{
    public class ActualizarPerfilCommand : IRequest<string>
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Correo { get; set; }

        public string Telefono { get; set; }

        public bool QuitarImagen { get; set; }

        public byte[] Imagen { get; set; }
    }
}