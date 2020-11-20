using System;
using System.Data.Common;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Infrastructure;
using Expertec.Sigeco.CrossCutting.EventBus.Abstractions;
using Expertec.Sigeco.CrossCutting.EventBus.Events;
using Expertec.Sigeco.CrossCutting.IntegrationEventLog.Services;
using Expertec.Sigeco.CrossCutting.IntegrationEventLog.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents
{
    public class AuthIntegrationEventService : IAuthIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly AuthContext _context;
        private readonly IIntegrationEventLogService _eventLogService;

        public AuthIntegrationEventService(Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory, IEventBus eventBus,
            AuthContext context)
        {
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventLogService = _integrationEventLogServiceFactory(_context.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception)
            {
                await _eventLogService.MarkEventAsFailedAsync(evt.Id);
            }            
        }

        public async Task SaveEventAndAuthContextChangesAsync(IntegrationEvent evt)
        {       
            await ResilientTransaction.New(_context)
                .ExecuteAsync(async () => {
                    await _context.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _context.Database.CurrentTransaction.GetDbTransaction());
                });
        }

        public async Task SaveEventAsync(IntegrationEvent evt)
        {
            await ResilientTransaction.New(_context)
                .ExecuteAsync(async () => {
                    await _eventLogService.SaveEventAsync(evt, _context.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}