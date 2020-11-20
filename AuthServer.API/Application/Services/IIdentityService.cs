namespace Expertec.Sigeco.AuthServer.API.Application.Services
{
    /// <summary>
    /// Interfaz para obtener información del token de acceso.
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// Devuelve el id de la empresa del usuario autenticado.
        /// </summary>
        int EmpresaId { get; }

        /// <summary>
        /// Devuelve el rol del usuario autenticado.
        /// </summary>
        string Rol { get; }

        /// <summary>
        /// Devuelve el nombre del usuario autenticado.
        /// </summary>
        string Usuario { get; }

        /// <summary>
        /// Devuelve el id del usuario autenticado.
        /// </summary>
        string UsuarioId { get; }

        /// <summary>
        /// Devuelve la aplicación cliente id.
        /// </summary>
        string AplicacionClienteId { get; }

        /// <summary>
        /// Devuelve Bearer Token
        /// </summary>
        string Token { get; }

    }

    
}