using System;
using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Autorizacion
    {
        #region Variables

        private readonly List<DetalleAutorizacion> _detalles;

        #endregion

        #region Propiedades

        public IReadOnlyCollection<DetalleAutorizacion> Detalles => _detalles;
        public int EmpresaId { get; set; }
        public int OpcionId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public Empresa Empresa { get; private set; }
        public Opcion Opcion { get; private set; }
        public Usuario Usuario { get; private set; }

        #endregion

        #region Constructor

        public Autorizacion()
        {
            _detalles = new List<DetalleAutorizacion>();
        }

        #endregion

        #region Metodos

        public void AgregarDetalle(DetalleAutorizacion item)
        {
            _detalles.Add(item);
        }

        #endregion

    }
}