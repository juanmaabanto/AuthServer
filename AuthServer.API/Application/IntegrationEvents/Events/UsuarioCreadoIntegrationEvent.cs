using Expertec.Sigeco.CrossCutting.EventBus.Events;

namespace Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents.Events
{
    public class UsuarioCreadoIntegrationEvent : IntegrationEvent
    {
        public string UsuarioId { get; }
        public string NombreUsuario { get; }
        public string Nombre { get; }
        public string Correo { get; }
        public bool Activo { get; }

        public UsuarioCreadoIntegrationEvent(string usuarioId, string nombreUsuario, string nombre, string correo, bool activo)
        {
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
            Nombre = nombre;
            Correo = correo;
            Activo = activo;
        }

    }

}