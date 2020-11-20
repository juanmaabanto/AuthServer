using System;

namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class OpcionTreeListDTO
    {
        public int OpcionId { get; set; }

        public int ModuloId { get; set; }

        public int? PadreId { get; set; }

        public string Nombre { get; set; }

        public string Tooltip { get; set; }

        public int Secuencia { get; set; }

        public string ViewClass { get; set; }

        public string ViewType { get; set; }

        public string Icono { get; set; }

        public bool Formulario { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Reactivo { get; set; }

        public string NombreHost { get; set; }

        public string Host { get; set; }

        public string Ruta { get; set; }

    }
}