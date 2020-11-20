using System;
using System.Collections.Generic;
using System.Linq;
using Expertec.Sigeco.AuthServer.API.SeedWork;

namespace Expertec.Sigeco.AuthServer.API.Models
{
    public class TipoEspacioTrabajo : Enumeration
    {
        
        #region Variables

        public static TipoEspacioTrabajo Production = new TipoEspacioTrabajo(1, nameof(Production).ToLowerInvariant());
        public static TipoEspacioTrabajo Test = new TipoEspacioTrabajo(2, nameof(Test).ToLowerInvariant());

        #endregion

        #region Constructor

        protected TipoEspacioTrabajo() {}

        protected TipoEspacioTrabajo(int tipoEspacioTrabajoId, string nombre ) : base(tipoEspacioTrabajoId, nombre) {}

        #endregion

        #region Metodos

        public static IEnumerable<TipoEspacioTrabajo> List() =>
            new [] { Production, Test };

        #endregion
    }
}