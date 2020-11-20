using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Expertec.Sigeco.AuthServer.API.Utils
{
    /// <summary>
    /// Metodos auxiliares.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Convierte enumerable de un objeto a arreglo de bytes.
        /// </summary>
        /// <param name="source">Listado fuente.</param>
        /// <typeparam name="TEntity">Tipo Entidad</typeparam>
        /// <returns>Devuelve arreglo de bytes para crear un archivo csv.</returns>
        public static byte[] GetBytesCSV<TEntity>(IEnumerable<TEntity> source) 
            where TEntity : class, new() => GetBytesCSV(source, false);

        /// <summary>
        /// Convierte enumerable de un objeto a arreglo de bytes.
        /// </summary>
        /// <param name="source">Listado fuente.</param>
        /// <param name="showIds">true para mostrar los ids de la entidad, en otro caso false.</param>
        /// <typeparam name="TEntity">Tipo Entidad</typeparam>
        /// <returns>Devuelve arreglo de bytes para crear un archivo csv.</returns>
        public static byte[] GetBytesCSV<TEntity>(IEnumerable<TEntity> source, bool showIds) 
            where TEntity : class, new()
        {
            var stringBuilder = new StringBuilder();
            var fields = typeof(TEntity).GetProperties().Where(p => !p.Name.EndsWith("Id") || showIds);

            stringBuilder.AppendLine(String.Join(",", fields.Select(f => f.Name)));

            foreach(var d in source)
            {
                stringBuilder.AppendLine(String.Join(",", fields.Select(f => f.GetValue(d))));
            }

            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        /// <summary>
        /// Convierte texto json en formato aceptado por Order By.
        /// </summary>
        /// <param name="sort">Texto en formato Json para formatear.</param>
        public static string GetOrderByFormat(string sort)
        {
            var orderby = string.Empty;

            if (sort != null)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(sort);

                foreach(var jsonElement in json.EnumerateArray())
                {
                    string property = jsonElement.GetProperty("property").ToString();
                    string direction = jsonElement.GetProperty("direction").ToString();

                    if ( !string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(direction) )
                    {
                        orderby += (string.IsNullOrEmpty(orderby) ? string.Empty : ", ") + property + " " + direction;
                    }
                }
            }

            return orderby;
        }
    }
}