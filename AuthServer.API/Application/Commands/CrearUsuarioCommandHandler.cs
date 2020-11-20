using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Expertec.Sigeco.AuthServer.API.Application.Services;
using Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.Utils;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Expertec.Sigeco.AuthServer.API.Grpc;
using Expertec.Sigeco.AuthServer.API.Application.IntegrationEvents.Events;
using Microsoft.EntityFrameworkCore;

namespace Expertec.Sigeco.AuthServer.API.Application.Commands
{
    public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, string>
    {
        private readonly IAuthIntegrationEventService _integrationEventService;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly IOptions<AuthSettings> _settings;

        public CrearUsuarioCommandHandler(IAuthIntegrationEventService integrationEventService, IEmpresaRepository empresaRepository, IPersonaRepository personaRepository,
            IIdentityService identityService, ILogger logger, UserManager<Usuario> userManager, IOptions<AuthSettings> settings)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _empresaRepository = empresaRepository ?? throw new ArgumentNullException(nameof(empresaRepository));
            _personaRepository = personaRepository ?? throw new ArgumentNullException(nameof(personaRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<string> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
        {
            int empresaId = _identityService.EmpresaId;
            string usuario = _identityService.Usuario;
            string token = _identityService.Token;

            try
            {
                if(request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                if(request.Empresas is null  || request.Empresas.Count() == 0)
                {
                    throw new AuthDomainException("No ha seleccionado empresas para el usuario.");
                }

                if(request.Empresas.Count(e => e.Principal) == 0)
                {
                    throw new AuthDomainException("No ha seleccionado empresa principal.");
                }

                //Agregar persona por Grpc
                var headers = new Metadata();
                headers.Add("Authorization", token);
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var httpClient = new HttpClient(httpClientHandler);
                var channel = GrpcChannel.ForAddress(_settings.Value.Services.AdminUrl, new GrpcChannelOptions { HttpClient = httpClient });
                var client = new Expertec.Sigeco.AuthServer.API.Grpc.Persona.PersonaClient(channel);

                var personaReply = await client.CrearSiNoExisteAsync(new PersonaRequest { 
                    TipoPersona = "N", 
                    PrimerNombre = request.PrimerNombre, 
                    SegundoNombre = request.SegundoNombre ?? string.Empty,
                    ApellidoPaterno = request.ApellidoPaterno,
                    ApellidoMaterno = request.ApellidoMaterno ?? string.Empty,
                    Dni = request.NroDocumento
                }, headers);

                var persona = await _personaRepository.ObtenerAsync(personaReply.PersonaId);
                
                if(persona == null)
                {
                    await _personaRepository.AgregarAsync(new Models.Persona {
                        ApellidoMaterno = personaReply.ApellidoMaterno,
                        ApellidoPaterno = personaReply.ApellidoPaterno,
                        NroDocumento = personaReply.NroDocumento,
                        PersonaId = personaReply.PersonaId,
                        PrimerNombre = personaReply.PrimerNombre,
                        SegundoNombre = personaReply.SegundoNombre
                    });
                }
                else
                {
                    persona.ApellidoMaterno = personaReply.ApellidoMaterno;
                    persona.ApellidoPaterno = personaReply.ApellidoPaterno;
                    persona.PrimerNombre = personaReply.PrimerNombre;
                    persona.SegundoNombre = personaReply.SegundoNombre;

                    await _personaRepository.ModificarAsync(persona);
                }

                //
                var newItem = new Usuario {
                    Activo = true,
                    Email = request.Correo.ToLowerInvariant(),
                    ExpiraClave = request.ExpiraClaveHabilitado ? DateTime.UtcNow.AddDays(30) : new DateTime?(),
                    ExpiraClaveHabilitado = request.ExpiraClaveHabilitado,
                    Nombre = request.Nombre,
                    PhoneNumber = request.Telefono,
                    RequiereCambioClave = true,
                    UserName = request.NombreUsuario
                };
                newItem.PersonaId = personaReply.PersonaId;

                using(var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var empresa = await _empresaRepository.ObtenerAsync(empresaId);

                        newItem.EspacioTrabajoId = empresa.EspacioTrabajoId;

                        foreach(var item in request.Empresas)
                        {
                            newItem.AgregarEmpresa(item.EmpresaId, item.Principal);
                        }

                        var result = await _userManager.CreateAsync(newItem, request.Clave);

                        if(!result.Succeeded)
                        {
                            throw new AuthDomainException(result.Errors.FirstOrDefault().Description);
                        }

                        result = await _userManager.SetLockoutEnabledAsync(newItem, request.BloqueoHabilitado);

                        if(!result.Succeeded)
                        {
                            throw new AuthDomainException(result.Errors.FirstOrDefault().Description);
                        }

                        result = await _userManager.AddToRoleAsync(newItem, "usuario");

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
                var evento = new UsuarioCreadoIntegrationEvent(newItem.Id, newItem.UserName, newItem.Nombre, newItem.Email, newItem.Activo);
                    
                await _integrationEventService.SaveEventAsync(evento);
                await _integrationEventService.PublishThroughEventBusAsync(evento);
                //
                return newItem.Id;   
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                var result = await _logger.ErrorAsync(ex.InnerException.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgPutError, result.EventLogId);
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, usuario);

                throw new AuthDomainException(Constants.MsgPostError, result.EventLogId);
            }
        }
    }
}