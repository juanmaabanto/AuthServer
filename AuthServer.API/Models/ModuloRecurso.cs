namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class ModuloRecurso
    {
        #region Propiedades

        public int ModuloId { get; set; }
        public int RecursoId { get; set; }
        public bool Activo { get; set; }
        public Modulo Modulo { get; private set; }
        public Recurso Recurso { get; private set; }

        #endregion

       
    }
}