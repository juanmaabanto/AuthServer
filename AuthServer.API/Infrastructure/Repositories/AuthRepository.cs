using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions;
using Expertec.Sigeco.AuthServer.API.Models;
using Expertec.Sigeco.AuthServer.API.SeedWork;
using Expertec.Sigeco.CrossCutting.LoggingEvent;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        #region Variables
        private readonly AuthContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<Usuario> _userManager;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Constructor

        public AuthRepository(AuthContext context, ILogger logger, UserManager<Usuario> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        #endregion

        #region Aplicacion Cliente

        public async Task<UsuarioAplicacionCliente> AgregarUsuarioAplicacionAsync(string usuarioId, string aplicacionId)
        {
            try
            {
                var data = await _context.UsuarioAplicacionesCliente.Where(p => p.UsuarioId == usuarioId && p.AplicacionClienteId == aplicacionId).FirstOrDefaultAsync();

                if(data != null)
                {
                    return data;
                }

                var item = (await _context.UsuarioAplicacionesCliente.AddAsync(new UsuarioAplicacionCliente(){
                    UsuarioId = usuarioId,
                    AplicacionClienteId = aplicacionId,
                    Fecha = DateTime.Now
                })).Entity;

                await _context.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error obteniendo datos.", result.EventLogId);
            }
        }

        public async Task<AplicacionCliente> GetAplicacionClienteAsync(string aplicacionClienteId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await _context.AplicacionesCliente.FindAsync(new object[] { aplicacionClienteId }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error al obtener aplicación cliente.", result.EventLogId);
            }
        }

        public async Task<dynamic> GetUsuarioAplicacionAsync(string usuario, string applicacionClienteId)
        {
            try
            {
                var app = await _context.AplicacionesCliente.Where(ac => ac.AplicacionClienteId == applicacionClienteId).FirstOrDefaultAsync();

                if(app == null)
                {
                    throw new AuthDomainException("El client_Id es invalido.");
                }

                var data = await (from u in _context.Usuarios
                    join a in _context.UsuarioAplicacionesCliente
                        on new { u.Id, applicacionClienteId } equals new { Id = a.UsuarioId, applicacionClienteId = a.AplicacionClienteId } into gj
                    from ua in gj.DefaultIfEmpty()
                    where u.UserName == usuario
                    select new {
                        NombreUsuario = u.UserName,
                        NombreAplicacion = app.Nombre,
                        Permitido = ua != null || !app.EsTercero
                    }).FirstOrDefaultAsync();

                return data ?? throw new KeyNotFoundException("Ocurrio un error obteniendo.");
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error obteniendo datos.", result.EventLogId);
            }
        }

        #endregion

        #region Empresa

        public async Task<UsuarioEmpresa> CompanyData(int empresaId, string usuarioId, string usuario)
        {
            var usuarioEmpresa = await _context.Set<UsuarioEmpresa>()
                    .Include(e => e.Empresa)
                    .Where(e => e.UsuarioId == usuarioId && e.EmpresaId == empresaId)
                    .FirstOrDefaultAsync();

            return usuarioEmpresa;
        }

        public IEnumerable<Empresa> GetCompanies(string userId, string userName)
        {
            IEnumerable<Empresa> result = null;

            try
            {
                result = (from e in _context.Set<Empresa>().AsEnumerable()
                    join ue in _context.Set<UsuarioEmpresa>().AsEnumerable() on e.EmpresaId equals ue.EmpresaId
                    where ue.UsuarioId == userId && e.Activo && !e.Anulado
                    select e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public async Task<int> GetEmpresaIdAsync(string usuarioId)
        {
            try
            {
                var usuarioEmpresa = await _context.UsuarioEmpresas
                    .Include(e => e.Empresa)
                    .Where(e => e.UsuarioId == usuarioId && e.Principal)
                    .FirstOrDefaultAsync();

                if (usuarioEmpresa == null)
                {
                    throw new AuthDomainException("El usuario no tiene una empresa asignada como principal.");
                }

                return usuarioEmpresa.EmpresaId;
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error al obtener información de la empresa.", result.EventLogId);
            }
        }

        public async Task<int> GetEmpresaIdAsync(string usuarioId, string codigo)
        {
            try
            {
                var usuarioEmpresa = await _context.UsuarioEmpresas
                    .Include(e => e.Empresa)
                    .Where(e => e.UsuarioId == usuarioId && e.Empresa.Codigo == codigo)
                    .FirstOrDefaultAsync();

                if (usuarioEmpresa == null)
                {
                    throw new AuthDomainException("El usuario no tiene acceso a la empresa o no existe");
                }

                return usuarioEmpresa.EmpresaId;
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error al obtener información de la empresa.", result.EventLogId);
            }
        }

        #endregion

        #region Usuario

        public async Task<Usuario> AutenticarAsync(string nombreUsuario, string clave, string aplicacionClienteId)
        {
            try
            {
                var usuario = await _userManager.FindByNameAsync(nombreUsuario);

                if(usuario is null)
                {
                    throw new AuthDomainException("El usuario no existe");
                }

                if(!usuario.Activo)
                {
                    throw new AuthDomainException("La cuenta del usuario se encuentra inactiva");
                }

                if(!string.IsNullOrEmpty(aplicacionClienteId))
                {
                    var aplicacion = await _context.UsuarioAplicacionesCliente.FindAsync(aplicacionClienteId);

                    if(!aplicacion.TieneAcceso)
                    {
                        throw new AuthDomainException($"No tiene acceso para la aplicación {aplicacionClienteId}");
                    }
                }  

                if(await _userManager.IsLockedOutAsync(usuario))
                {
                    var tiempoRestante = (usuario.LockoutEnd.Value.DateTime - DateTime.UtcNow).ToString(@"mm\m\ ss\s\ ");
                    throw new AuthDomainException($"El usuario se encuentra bloqueado. {tiempoRestante} restantes");
                }

                var resultado = await _userManager.CheckPasswordAsync(usuario, clave);

                if(resultado)
                {
                    await _userManager.ResetAccessFailedCountAsync(usuario);
                    
                    return usuario;
                }
                else
                {
                    var intentos = _userManager.Options.Lockout.MaxFailedAccessAttempts - await _userManager.GetAccessFailedCountAsync(usuario);
                    await _userManager.AccessFailedAsync(usuario);

                    if(intentos <= 1)
                    {
                        var tiempoRestante = (usuario.LockoutEnd.Value.DateTime - DateTime.UtcNow).ToString(@"mm\m\ ss\s\ ");

                        throw new AuthDomainException($"Contraseña incorrecta. Cuenta Bloqueada. {tiempoRestante} restantes");
                    }
                    else
                    {
                        throw new AuthDomainException($"Contraseña incorrecta. {intentos - 1} intento(s) restante(s).");
                    }
                }
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace, nombreUsuario);

                throw new AuthDomainException("Ocurrio un error al intentar autenticar.", result.EventLogId);
            }
        }
        public async Task<bool> CambiarClaveAsync(string nombreUsuario, string clave, string nuevaClave)
        {
            try
            {
                var usuario = await _userManager.FindByNameAsync(nombreUsuario);

                if(usuario is null)
                {
                    throw new AuthDomainException("El usuario no existe");
                }

                var claves = await  _context.HistoricoClaves.Where(c => c.UsuarioId == usuario.Id).OrderByDescending(c => c.FechaRegistro).Take(2).ToListAsync();

                foreach(var c in claves)
                {
                    if(_userManager.PasswordHasher.VerifyHashedPassword(usuario, c.Clave, nuevaClave) == PasswordVerificationResult.Success)
                    {
                        throw new AuthDomainException("La nueva contraseña debe ser diferente a las últimas dos.");
                    }
                }

                var resultado = await _userManager.ChangePasswordAsync(usuario, clave, nuevaClave);

                if(resultado.Succeeded)
                {
                    var historico = new HistoricoClave() {
                        Clave = usuario.PasswordHash,
                        FechaRegistro = DateTime.Now,
                        UsuarioId = usuario.Id
                    };

                    await _context.HistoricoClaves.AddAsync(historico);
                    await _context.SaveChangesAsync();

                    if(usuario.ExpiraClaveHabilitado)
                    {
                        usuario.ExpiraClave = DateTime.UtcNow.AddDays(90);
                    }

                    usuario.RequiereCambioClave = false;
                    await _userManager.UpdateSecurityStampAsync(usuario);
                    return true;
                }
                else
                {
                    switch (resultado.Errors.FirstOrDefault().Code)
                    {
                        case nameof(IdentityErrorDescriber.InvalidEmail):
                            throw new AuthDomainException("El correo electrónico es inválido.");
                        case nameof(IdentityErrorDescriber.PasswordMismatch):
                            throw new AuthDomainException("Contraseña incorrecta.");
                        case nameof(IdentityErrorDescriber.PasswordTooShort):
                            throw new AuthDomainException("Las contraseñas deben tener al menos 8 caracteres.");
                        case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
                            throw new AuthDomainException("Las contraseñas deben tener al menos un carácter no alfanumérico.");
                        case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
                            throw new AuthDomainException("Las contraseñas deben tener al menos un dígito ('0' - '9').");
                        case nameof(IdentityErrorDescriber.PasswordRequiresLower):
                            throw new AuthDomainException("Las contraseñas deben tener al menos una minúscula ('a' - 'z').");
                        case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
                            throw new AuthDomainException("Las contraseñas deben tener al menos una mayúscula ('A' - 'Z').");
                        default:
                            throw new AuthDomainException(resultado.Errors.FirstOrDefault().Description);
                    }
                }
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error al intentar cambiar clave.", result.EventLogId);
            }
        }
        public async Task<string> GetRolAsync(string usuarioId)
        {
            try
            {
                var usuarioRol = await _context.UsuarioRoles
                    .Include(r => r.Rol)
                    .Where(u => u.UserId == usuarioId)
                    .FirstOrDefaultAsync();
                
                if(usuarioRol == null)
                {
                    throw new AuthDomainException("El usuario no tiene un rol asignado");

                }

                return usuarioRol.Rol.Name;
            }
            catch(AuthDomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error al intentar obtener datos del rol.", result.EventLogId);
            }
        }
        public async Task<dynamic> GetUsuarioAsync(string nombreUsuario)
        {
            try
            {
                var login = await _context.Set<Usuario>()
                    .Where(u => u.UserName == nombreUsuario)
                    .Select(u => new {
                        Avatar = (u.Imagen == null ? null : Convert.ToBase64String(u.Imagen)),
                        Nombre = u.Nombre,
                        NombreUsuario = u.UserName,
                        UsuarioId = u.Id
                    }).FirstOrDefaultAsync();

                return login ?? throw new KeyNotFoundException("No pudimos encontrar tu cuenta.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error obteniendo datos de tu cuenta.", result.EventLogId);
            }
        }
        public async Task<Usuario> GetUsuarioPorIdAsync(string usuarioId)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Where(u => u.Id == usuarioId)
                    .FirstOrDefaultAsync();

                return usuario ?? throw new KeyNotFoundException("No pudimos encontrar tu cuenta.");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var result = await _logger.ErrorAsync(ex.Message, ex.Source, ex.StackTrace);

                throw new AuthDomainException("Ocurrio un error obteniendo datos de tu cuenta.", result.EventLogId);
            }
        }
        public async Task<int> SaveAuthorizationAsync(List<Dictionary<string, object>> lst, string userId, byte companyId, string userName)
        {
            int result = 0;

            try
            {
                foreach (var item in lst)
                {
                    var entity = await _context.Set<Autorizacion>().Where(p => p.UsuarioId == userId && p.EmpresaId == companyId && p.OpcionId == Convert.ToInt16(item["optionId"].ToString())).FirstOrDefaultAsync();

                    if (Convert.ToInt16(item["detailOptionId"].ToString()) == 0)
                    {
                        if (entity == null)
                        {
                            if(Convert.ToBoolean(item["allowed"].ToString())) {
                                entity = new Autorizacion()
                                {
                                    UsuarioId = userId,
                                    OpcionId = Convert.ToInt16(item["optionId"].ToString()),
                                    EmpresaId = companyId,
                                    FechaInicio = Convert.ToDateTime(item["startDate"].ToString()).Date,
                                    FechaFin = Convert.ToDateTime(item["endDate"].ToString()).Date,
                                    FechaRegistro = DateTime.Now,
                                    UsuarioRegistro = userName
                                };
                                await _context.Set<Autorizacion>().AddAsync(entity);
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(item["allowed"].ToString()))
                            {
                                entity.FechaInicio = Convert.ToDateTime(item["startDate"].ToString()).Date;
                                entity.FechaFin = Convert.ToDateTime(item["endDate"].ToString()).Date;
                                entity.UsuarioModificacion = userName;
                                entity.FechaModificacion = DateTime.Now;

                                _context.Set<Autorizacion>().Update(entity);
                            }
                            else
                            {
                                entity.Detalles.ToList().ForEach(d => { _context.Set<DetalleAutorizacion>().Remove(d); });
                                _context.Set<Autorizacion>().Remove(entity);
                            }
                        }
                    }
                    else
                    {
                        var detail = await _context.Set<DetalleAutorizacion>().Where(p => p.EmpresaId == companyId && p.UsuarioId == userId 
                            && p.OpcionId == Convert.ToInt16(item["optionId"].ToString()) && p.DetalleOpcionId == Convert.ToInt16(item["detailOptionId"].ToString())).FirstOrDefaultAsync();

                        if (detail == null)
                        {
                            if(Convert.ToBoolean(item["allowed"].ToString())) 
                            {
                                await _context.Set<DetalleAutorizacion>().AddAsync(new DetalleAutorizacion()
                                {
                                    UsuarioId = userId,
                                    OpcionId = Convert.ToInt16(item["optionId"].ToString()),
                                    EmpresaId = companyId,
                                    DetalleOpcionId = Convert.ToInt16(item["detailOptionId"].ToString()),
                                    FechaInicio = Convert.ToDateTime(item["startDate"].ToString()).Date,
                                    FechaFin = Convert.ToDateTime(item["endDate"].ToString()).Date,
                                    FechaRegistro = DateTime.Now,
                                    UsuarioRegistro = userName
                                });
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(item["allowed"].ToString()))
                            {
                                detail.FechaInicio = Convert.ToDateTime(item["startDate"].ToString()).Date;
                                detail.FechaFin = Convert.ToDateTime(item["endDate"].ToString()).Date;
                                detail.FechaModificacion = DateTime.Now;
                                detail.UsuarioModificacion = userName;
                                
                                _context.Set<DetalleAutorizacion>().Update(detail);
                            }
                            else
                            {
                                _context.Set<DetalleAutorizacion>().Remove(detail);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                result = 1;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return result;
        }

        public bool IsAuthorize(string userId, int companyId, string controller, string action, string aplicacionClienteId)
        {
            bool result = false;

            try
            {
                var details = (from d in _context.Set<DetalleOpcion>()
                    join o in _context.Set<Opcion>() on d.OpcionId equals o.OpcionId
                    join m in _context.Set<Modulo>() on o.ModuloId equals m.ModuloId
                    where d.NombreControlador == controller && d.NombreAccion == action && d.Activo == true
                        && o.Activo && m.Activo && m.AplicacionClienteId == aplicacionClienteId
                    select d
                );

                foreach(var detail in details)
                {
                    int optionId = detail.OpcionId;
                    int detailOptionId = detail.DetalleOpcionId;

                    var authorize = _context.Set<DetalleAutorizacion>().Where(auth => auth.UsuarioId == userId && auth.EmpresaId == companyId && auth.OpcionId == optionId &&
                        auth.DetalleOpcionId == detailOptionId && auth.FechaFin > DateTime.UtcNow).FirstOrDefault();

                    if (authorize != null)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return result;
        }

        
        #endregion
    }
}