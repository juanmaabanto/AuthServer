namespace Expertec.Sigeco.AuthServer.API.ViewModel
{
    /// <summary>
    /// Modelo de error devuelto al usuario.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtiene el id del registro del evento.
        /// </summary>
        public int EventLogId { get; }

        /// <summary>
        /// Obtiene el mensaje de error.
        /// </summary>
        /// <value></value>
        public string Message { get; }

        /// <summary>
        /// Crea un nuevo modelo de error para mostrar al usuario.
        /// </summary>
        /// <param name="eventLogId">Id del registro.</param>
        /// <param name="message">Mensaje para mostrar.</param>
        public ErrorViewModel(int eventLogId, string message)
        {
            EventLogId = eventLogId;
            Message = message;
        }
    }
}