namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Favorito
    {
        #region Propiedades

        public int EmpresaId { get; set; }
        public int OpcionId { get; set; }
        public string UsuarioId { get; set; }
        public bool Activo { get; set; }
        public Empresa Empresa { get; private set; }
        public Opcion Opcion { get; private set; }
        public Usuario Usuario { get; private set; }

        #endregion

       
    }
}