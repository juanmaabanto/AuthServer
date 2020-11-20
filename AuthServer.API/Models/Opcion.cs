using System;
using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Opcion
    {
        #region Variables

        private readonly List<DetalleOpcion> _detalles;

        #endregion

        #region  Propiedades

        public IReadOnlyCollection<DetalleOpcion> Detalles => _detalles;

        public int OpcionId { get; set; }

        public int ModuloId { get; set; }

        public int? PadreId { get; set; }

        public string Nombre { get; set; }

        public string Tooltip { get; set; }

        public int Secuencia { get; set; }

        public string ViewClass { get; set; }

        public string ViewType { get; set; }

        public string Icono { get; set; }

        public bool Formulario { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Reactivo { get; set; }

        public string Ruta { get; set; }

        public Opcion Padre { get; private set; }

        #endregion

        #region Constructor

        public Opcion()
        {
            _detalles = new List<DetalleOpcion>();
        }

        #endregion

        #region Metodos

        public void AgregarDetalle(DetalleOpcion item)
        {
            _detalles.Add(item);
        }

        #endregion
    }
}