using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class DetalleAutorizacion
    {
        #region Propiedades

        public int EmpresaId { get; set; }
        public int OpcionId { get; set; }
        public string UsuarioId { get; set; }
        public int DetalleOpcionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DetalleOpcion DetalleOpcion { get; private set; }

        #endregion

    }

}