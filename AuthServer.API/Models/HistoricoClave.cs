using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class HistoricoClave
    {
        #region Propiedades

        public int HistoricoClaveId { get; set; }
        public string UsuarioId { get; set; }
        public string Clave { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Usuario Usuario { get; set; }

        #endregion

    }

}