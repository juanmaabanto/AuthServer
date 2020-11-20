using System.Threading.Tasks;
using Expertec.Sigeco.CrossCutting.EventBus.Events;

namespace Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents
{
    public interface IAuthIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);

        Task SaveEventAndAuthContextChangesAsync(IntegrationEvent evt);

        Task SaveEventAsync(IntegrationEvent evt);
    }
}