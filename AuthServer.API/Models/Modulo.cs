using System;
using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class Modulo
    {
        #region "Variables"

        private readonly List<Opcion> _opciones;

        #endregion

        #region "Propiedades"

        public IReadOnlyCollection<Opcion> Opciones => _opciones;
        
        public int ModuloId { get; set; }

        public string AplicacionClienteId { get; set; }

        public string Nombre { get; set; }

        public string NombreCorto { get; set; }

        public string NombreRuta { get; set; }

        public string UriRuta { get; set; }

        public string Imagen { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioRegistro { get; set; }
        
        public string NombreHost { get; set; }

        public string Host { get; set; }

        #endregion

        #region "Constructor"

        public Modulo()
        {
            _opciones = new List<Opcion>();
        }

        #endregion

        #region Foreign Keys
        
        public AplicacionCliente AplicacionCliente { get; private set; }

        #endregion

        #region Metodos

        public void AgregarOpcion(Opcion item)
        {
            _opciones.Add(item);
        }

        #endregion
    }
}