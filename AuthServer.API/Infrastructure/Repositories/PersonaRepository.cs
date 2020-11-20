using System;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        #region Variables

        private readonly AuthContext _ctx;

        #endregion

        #region Constructor

        public PersonaRepository(AuthContext context)
        {
            _ctx = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Metodos

        public async Task AgregarAsync(Persona item)
        {
            await _ctx.AddAsync(item);
            await _ctx.SaveChangesAsync();
        }

        public async Task ModificarAsync(Persona item)
        {
            _ctx.Update(item);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Persona> ObtenerAsync(int personaId)
        {
            return await _ctx.Set<Persona>().FirstOrDefaultAsync(p => p.PersonaId == personaId);
        }

        #endregion

        #region "Dispose"

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ctx != null)
                {
                    _ctx.Dispose();
                }
            }
        }

        #endregion

    }
}