namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class AplicacionCliente
    {
        #region Propiedades

        public string AplicacionClienteId { get; set; }
        public string Secreto { get; set; }
        public string Nombre { get; set; }
        public int TipoAplicacion { get; set; }
        public int TiempoVidaTokenActualizacion { get; set; }
        public string OrigenPermitido { get; set; }
        public string RedirigirUri { get; set; }
        public bool EsTercero { get; set; }
        public bool Activo { get; set; }

        #endregion
        
    }
}