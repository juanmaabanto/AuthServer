using System;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Persona
    {
        public int PersonaId { get; set; }
        public string NroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
    }

}