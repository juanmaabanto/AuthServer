namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class DetalleOpcion
    {
        #region Propiedades

        public int DetalleOpcionId { get; set; }
        public int OpcionId { get; set; }
        public string Nombre { get; set; }
        public string NombreControlador { get; set; }
        public string NombreAccion { get; set; }
        public bool Activo { get; set; }

        #endregion

       
    }
}