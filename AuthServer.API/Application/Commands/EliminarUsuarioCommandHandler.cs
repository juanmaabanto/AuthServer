using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents;
using Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents.Events;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.Utils;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Expertec.Sigeco.AuthServer.API.Application.Commands
{
    public class EliminarUsuarioCommandHandler : IRequestHandler<EliminarUsuarioCommand, bool>
    {
        private readonly IAuthIntegrationEventService _integrationEventService;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;
        private readonly UserManager<Usuario> _userManager;

        public EliminarUsuarioCommandHandler(IAuthIntegrationEventService integrationEventService, IIdentityService identityService, 
            ILogger logger, UserManager<Usuario> userManager)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<bool> Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
        {
            string usuario = _identityService.Usuario;

            try
            {
                using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var user = await _userManager.FindByIdAsync(request.UsuarioId);

                        if(user == null)
                        {
                            throw new KeyNotFoundException(nameof(user));
                        }

                        var roles = await _userManager.GetRolesAsync(user);
                        var result = await _userManager.RemoveFromRolesAsync(user, roles);

                        if(!result.Succeeded)
                        {
                            throw new AuthDomainException(result.Errors.FirstOrDefault().Description);
                        }

                        result = await _userManager.DeleteAsync(user);

                        if(!result.Succeeded)
                        {
                            throw new AuthDomainException(result.Errors.FirstOrDefault().Description);
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                        transaction.Complete();
                    }
                    catch (Exception)
                    {
                        transaction.Dispose();
                        throw;
                    }
                }
                //Integración de eventos
                var evento = new UsuarioEliminadoIntegrationEvent(request.UsuarioId);
                
                await _integrationEventService.SaveEventAsync(evento);
                await _integrationEventService.PublishThroughEventBusAsync(evento);
                //
                return true;
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch(KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgDeleteError, result.EventLogId);
            }
        }
    }
}