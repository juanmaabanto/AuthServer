using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public interface IModuloRepository : IDisposable
    {
        #region Detalle Autorización

        Task<IEnumerable<object>> ObtenerAccesoAccionAsync(int opcionId);

        #endregion

        #region Favoritos

        Task<Boolean> AgregarFavoritoAsync(int opcionId);
        Task<Boolean> QuitarFavoritoAsync(int opcionId);

        #endregion
    }
}