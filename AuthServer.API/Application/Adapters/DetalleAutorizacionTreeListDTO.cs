using System;

namespace Expertec.Sigeco.AuthServer.API.Application.Adapters
{
    public class DetalleAutorizacionTreeListDTO
    {

        public int EmpresaId { get; set; }

        public string UsuarioId { get; set; }

        public int OpcionId { get; set; }

        public int DetalleOpcionId { get; set; }

        public string Nombre { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

    }
}