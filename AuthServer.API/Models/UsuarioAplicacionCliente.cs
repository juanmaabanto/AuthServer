using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class UsuarioAplicacionCliente
    {
        #region Propiedades

        public string UsuarioId { get; set; }
        public string AplicacionClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Permitido { get; set; }
        public bool TieneAcceso { get; set; }

        #endregion

        #region Foreign Keys

        public Usuario Usuario { get; private set; }
        public AplicacionCliente AplicacionCliente { get; private set; }

        #endregion
    }
}