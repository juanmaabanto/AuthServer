using Expertec.Sigeco.CrossCutting.EventBus.Events;

namespace Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents.Events
{
    public class UsuarioEliminadoIntegrationEvent : IntegrationEvent
    {
        public string UsuarioId { get; }

        public UsuarioEliminadoIntegrationEvent(string usuarioId)
        {
            UsuarioId = usuarioId;
        }

    }

}