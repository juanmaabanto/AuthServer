using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class DetalleEspacioTrabajo
    {

        #region "Propiedades"

        public int DetalleEspacioTrabajoId { get; set; }
        public int EspacioTrabajoId { get; set; }
        public int ModuloId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        public EspacioTrabajo EspacioTrabajo { get; private set; }
        public Modulo Modulo { get; private set; }

        #endregion

    }

}