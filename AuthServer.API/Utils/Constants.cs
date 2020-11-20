namespace Expertec.Sigeco.AuthServer.API.Utils
{
    public static class Constants
    {
        public const string MsgUnauthenticate = "{ \"message\":\"No se encuentra autenticado o su sesión a expirado.\" }";
        public const string MsgUnauthorize = "{ \"message\":\"No tiene autorización para acceder al recurso solicitado.\" }";

        public const string MsgGetError = "Ha ocurrido un error obteniendo la información.";
        public const string MsgPostError = "Ha ocurrido un error al intentar registrar la información.";
        public const string MsgPutError = "Ha ocurrido un error al intentar actualizar la información.";
        public const string MsgDeleteError = "Ha ocurrido un error al eliminar.";
        
        public const string MsgDataNotFound = "No hay información para mostrar.";
        public const string MsgCodeCompany = "Error al guardar, ya existe una compañia con el mismo código.";
    }
}