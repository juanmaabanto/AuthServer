namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class UsuarioEmpresa
    {
        #region Propiedades
        public string UsuarioId { get; set; }
        public int EmpresaId { get; set; }
        public bool Principal { get; set; }
        public Empresa Empresa { get; private set; }

        #endregion
        
    }
}