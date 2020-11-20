using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class EspacioTrabajo
    {
        #region "Propiedades"

        public int EspacioTrabajoId { get; set; }
        public int TipoEspacioTrabajoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Dominio { get; set; }
        public string Contacto { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activo { get; set; }
        public TipoEspacioTrabajo TipoEspacioTrabajo { get; private set; }

        #endregion

    }
}