using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class TokenActualizacion
    {

        #region Propiedades

        public string TokenActualizacionId { get; set; }
        public string AplicacionClienteId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public byte[] TicketProtegido { get; set; }
        public AplicacionCliente AplicacionCliente { get; private set; }
        public Usuario Usuario { get; private set; }

        #endregion
    }
}