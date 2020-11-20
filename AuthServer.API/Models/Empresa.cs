using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Empresa
    {
        #region Propiedades

        public int EmpresaId { get; set; }
        public int EspacioTrabajoId { get; set; }
        public string Codigo { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string DomicilioFiscal { get; set; }
        public string NombreComercial { get; set; }
        public string Ubigeo { get; set; }
        public string CuentaDetraccion { get; set; }
        public string Logo { get; set; }
        public string ImagenUri { get; set; }
        public bool ExoneradoIGV { get; set; }
        public bool IncluyeIGV { get; set; }
        public bool Activo { get; set; }
        public bool Anulado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public EspacioTrabajo EspacioTrabajo { get; private set; }

        #endregion

    }
}