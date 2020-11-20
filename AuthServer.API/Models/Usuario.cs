using System;
using System.Collections.Generic;
using Expertec.Sigeco.AuthServer.API.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    /// <summary>
    /// Entidad Usuario
    /// </summary>
    public class Usuario : IdentityUser, IAggregateRoot
    {
        #region Variables

        private readonly List<UsuarioEmpresa> _empresas;
        private readonly List<IdentityUserClaim<string>> _reclamaciones;
        private readonly List<UsuarioRol> _roles;

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene lista de empresas a las que tiene acceso el usuario.
        /// </summary>
        public IReadOnlyCollection<UsuarioEmpresa> Empresas => _empresas;

        /// <summary>
        /// Obtiene lista de reclamaciones del usuario.
        /// </summary>
        public IReadOnlyCollection<IdentityUserClaim<string>> Reclamaciones => _reclamaciones;

        /// <summary>
        /// Obtiene lista de roles del usuario.
        /// </summary>
        public IReadOnlyCollection<UsuarioRol> Roles => _roles;

        /// <summary>
        /// Obtiene el id del espacio de trabajo al que pertenece el usuario.
        /// </summary>
        public int EspacioTrabajoId { get; set; }

        /// <summary>
        /// Obtiene el id de la persona asignada al usuario.
        /// </summary>
        public int PersonaId { get; set; }

        /// <summary>
        /// Obtiene el nombre para mostrar del usuario.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene la imagen para mostrar del usuario.
        /// </summary>
        public byte[] Imagen { get; set; }

        /// <summary>
        /// Fecha de vencimiento de clave en UTC.
        /// </summary>
        /// <value></value>
        public DateTimeOffset? ExpiraClave { get; set; }

        /// <summary>
        /// Indica si la contraseña del usuario expira.
        /// </summary>
        /// <value></value>
        public bool ExpiraClaveHabilitado { get; set; }

        /// <summary>
        /// Indica si el usuario tiene que cambiar la clave, la próxima vez que inicie sesión.
        /// </summary>
        public bool RequiereCambioClave { get; set; }

        /// <summary>
        /// Indica si el usuario esta activo para iniciar sesión.
        /// </summary>
        /// <value>True si el usuario puede iniciar sesión, en otro caso false.</value>
        public bool Activo { get; set; }

        /// <summary>
        /// Obtiene la entidad del espacio de trabajo al que pertenece el usuario.
        /// </summary>
        /// <value></value>
        public EspacioTrabajo EspacioTrabajo { get; private set; }

        /// <summary>
        /// Obtiene la persona asignada al usuario.
        /// </summary>
        /// <value></value>
        public Persona Persona { get; private set; }

        #endregion

        #region Constructor

        public Usuario()
        {
            _empresas = new List<UsuarioEmpresa>();
            _reclamaciones = new List<IdentityUserClaim<string>>();
            _roles = new List<UsuarioRol>();
        }

        #endregion

        #region Metodos

        public void AgregarEmpresa(int empresaId, bool principal)
        {
            if(_empresas.Exists(e => e.EmpresaId == empresaId))
            {
                _empresas.Find(e => e.EmpresaId == empresaId).Principal = principal;
            }
            else
            {
                var usuarioEmpresa = new UsuarioEmpresa(){ UsuarioId = Id, EmpresaId = empresaId, Principal = principal };

                _empresas.Add(usuarioEmpresa);
            }
        }

        public void AgregarRol(string rolId)
        {
            var usuarioRol = new UsuarioRol() { RoleId = rolId, UserId = Id };

            _roles.Add(usuarioRol);
        }

        #endregion
        
    }
}