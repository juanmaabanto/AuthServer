namespace Expertec.Sigeco.AuthServer.API.ViewModel
{
    /// <summary>
    /// Modelo de error por validación devuelto al usuario.
    /// </summary>
    public class ValidatorViewModel
    {
        /// <summary>
        /// Obtiene el nombre de la propiedad a validar
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Obtiene el mensaje de error de validación.
        /// </summary>
        /// <value></value>
        public string Message { get; }

        /// <summary>
        /// Crea un nuevo modelo de error de validación para mostrar al usuario.
        /// </summary>
        /// <param name="property">Nombre de la propiedad.</param>
        /// <param name="message">Mensaje para mostrar.</param>
        public ValidatorViewModel(string property, string message)
        {
            Property = property;
            Message = message;
        }
    }
}