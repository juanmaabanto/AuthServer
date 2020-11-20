using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Expertec.Sigeco.AuthServer.API.Application.Commands
{
    public class EliminarUsuarioCommand : IRequest<bool>
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string UsuarioId { get; set; }
    }
}