using System;
using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class AutorizacionTreeListDTO
    {

        public int EmpresaId { get; set; }

        public string UsuarioId { get; set; }

        public int OpcionId { get; set; }

        public int? PadreId { get; set; }

        public string Nombre { get; set; }

        public string Icono { get; set; }

        public bool Formulario { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public List<DetalleAutorizacionTreeListDTO> Detalles { get; set; }

    }
}