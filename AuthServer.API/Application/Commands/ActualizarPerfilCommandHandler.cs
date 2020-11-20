using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class ActualizarPerfilCommandHandler : IRequestHandler<ActualizarPerfilCommand, string>
    {
        private readonly IAuthIntegrationEventService _integrationEventService;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;
        private readonly UserManager<Usuario> _userManager;

        public ActualizarPerfilCommandHandler(IAuthIntegrationEventService integrationEventService, IIdentityService identityService, 
            ILogger logger, UserManager<Usuario> userManager)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<string> Handle(ActualizarPerfilCommand request, CancellationToken cancellationToken)
        {
            string usuario = _identityService.Usuario;
            string usuarioId = _identityService.UsuarioId;

            try
            {
                if(usuarioId != request.UsuarioId)
                {
                    throw new AuthDomainException("No es posible actualizar el perfil de otro usuario.");
                }
                
                bool cambia = false;
                var current = await _userManager.FindByIdAsync(request.UsuarioId);

                if(current == null)
                {
                    throw new KeyNotFoundException(nameof(request));
                }

                if(current.Nombre != request.Nombre || current.Email != request.Correo)
                {
                    cambia = true;
                }

                current.Nombre = request.Nombre;
                current.Email = request.Correo.ToLowerInvariant();
                current.PhoneNumber = request.Telefono;

                if(request.QuitarImagen)
                {
                    current.Imagen = null;
                }
                else
                {
                    if(request.Imagen != null)
                    {
                        current.Imagen = request.Imagen;
                    }
                }

                var result = await _userManager.UpdateAsync(current);

                if(!result.Succeeded)
                {
                    throw new AuthDomainException(result.Errors.FirstOrDefault().Description);
                }

                //Integración de eventos
                if(cambia)
                {
                    var evento = new UsuarioModificadoIntegrationEvent(current.Id, current.Nombre, current.Email, current.Activo);
                
                    await _integrationEventService.SaveEventAsync(evento);
                    await _integrationEventService.PublishThroughEventBusAsync(evento);
                }
                //
                return current.Id;
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

                throw new AuthDomainException(Constants.MsgPutError, result.EventLogId);
            }
        }
    }
}