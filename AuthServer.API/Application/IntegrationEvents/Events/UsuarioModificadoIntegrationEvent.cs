using Expertec.Sigeco.CrossCutting.EventBus.Events;

namespace Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents.Events
{
    public class UsuarioModificadoIntegrationEvent : IntegrationEvent
    {
        public string UsuarioId { get; }
        public string Nombre { get; }
        public string Correo { get; }
        public bool Activo { get; }

        public UsuarioModificadoIntegrationEvent(string usuarioId, string nombre, string correo, bool activo)
        {
            UsuarioId = usuarioId;
            Nombre = nombre;
            Correo = correo;
            Activo = activo;
        }

    }

}