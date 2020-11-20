using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Application.Adapters;
using Expertec.Sigeco.AuthServer.API.Application.Commands;
using Expertec.Sigeco.AuthServer.API.Application.Queries;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Expertec.Sigeco.AuthServer.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class UsuariosController : Controller
    {
        #region "Variables"
        private readonly IMediator _mediator;
        private readonly IUsuarioQueries _queries;

        #endregion

        #region "Constructor"

        public UsuariosController(IMediator mediator, IUsuarioQueries queries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        #endregion

        #region Gets

        /// <summary>
        /// Lista de usuarios.
        /// </summary>
        /// <param name="nombre">Nombres del usuario.</param>
        /// <param name="sort"></param>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(PaginatedViewModel<IEnumerable<UsuarioListDTO>>), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Listar(string nombre, string sort, [FromQuery]int pageSize = 50, [FromQuery]int start = 0,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _queries.ListarAsync(nombre, sort, pageSize, start, cancellationToken);

                var model = new PaginatedViewModel<UsuarioListDTO>(
                start, pageSize, result.total, result.data);

                return Ok(model);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        /// <summary>
        /// Devuelve datos de usuario.
        /// </summary>
        /// <param name="usuarioId">Id del usuario.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{usuarioId}")]
        [ProducesResponseType(typeof(UsuarioDTO), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Obtener(string usuarioId)
        {
            try
            {
                var result = await _queries.ObtenerAsync(usuarioId);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Devuelve listado en un archivo csv.
        /// </summary>
        /// <param name="nombre">Nombres del usuario.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("csv")]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<ActionResult> ObtenerCSV(string nombre)
        {
            try
            {
                var result = await _queries.ObtenerBytesAsync(nombre);

                return File(result, "text/csv", $"usuarios_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv");
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        /// <summary>
        /// Obtener perfil de la sesión.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("perfil")]
        [ProducesResponseType(typeof(PerfilDTO), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var result = await _queries.ObtenerPerfilAsync();

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtener perfil de usuario.
        /// </summary>
        /// <param name="usuarioId">Id del usuario.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{usuarioId}/perfil")]
        [ProducesResponseType(typeof(PerfilDTO),200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(404)]     
        public async Task<IActionResult> PerfilPorUsuario(string usuarioId)
        {
            try
            {
                var result = await _queries.ObtenerPerfilAsync(usuarioId);

                return Ok(result);
            }
            catch (AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        #endregion

        #region Posts

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Usuario), 201)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Agregar([FromBody]CrearUsuarioCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                return CreatedAtAction(nameof(Obtener), new { usuarioId = result }, result);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
        }

        #endregion

        #region Puts

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(Usuario), 201)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Modificar([FromBody]ActualizarUsuarioCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                return CreatedAtAction(nameof(Obtener), new { usuarioId = result }, result);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("perfil")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> ModificarPerfil(IFormFile file, [FromForm]ActualizarPerfilCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if(command == null)
                {
                    throw new ArgumentNullException(nameof(command));
                }

                if(file == null || file.Length == 0)
                {
                    if(command.QuitarImagen)
                    {
                        command.Imagen = null;
                    }
                }
                else
                {
                    if(file.Length > 204800)
                    {
                        throw new AuthDomainException("La imagén es muy grande(tamaño máximo de 200kb).");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);

                        command.Imagen = memoryStream.ToArray();
                    }
                }

                var result = await _mediator.Send(command, cancellationToken);

                return CreatedAtAction(nameof(PerfilPorUsuario), new { usuarioId = result }, result);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(new ErrorViewModel(0, ex.Message));
            }
        }

        #endregion

        #region Deletes

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        [Route("{usuarioId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        public async Task<IActionResult> Eliminar(string usuarioId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var command = new EliminarUsuarioCommand{ UsuarioId = usuarioId };
                var result = await _mediator.Send(command, cancellationToken);

                return Ok(true);
            }
            catch(AuthDomainException ex)
            {
                return BadRequest(new ErrorViewModel(ex.ErrorId, ex.Message));
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion
    }
}