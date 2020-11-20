using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Recurso
    {
        #region Propiedades

        public int RecursoId { get; set; }
        public string Nombre { get; set; }
        public string NombreHost { get; set; }
        public string UriHost { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }


        #endregion

       
    }
}