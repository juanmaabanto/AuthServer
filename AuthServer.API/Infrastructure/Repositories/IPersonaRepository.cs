using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Expertec.Sigeco.AuthServer.API.Models;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Repositories
{
    public interface IPersonaRepository : IDisposable
    {
        Task<Persona> ObtenerAsync(int personaId);
        Task AgregarAsync(Persona item);
        Task ModificarAsync(Persona item);
    }
}