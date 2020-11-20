using System.Collections.Generic;

namespace Expertec.Sigeco.AuthServer.API.ViewModel
{
    /// <summary>
    /// Modelo de paginación para listados.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PaginatedViewModel<TEntity> where TEntity : class
    {
        public int Start { get; }

        public int PageSize { get; }

        public long Total { get; }

        public IEnumerable<TEntity> Data { get; }

        public PaginatedViewModel(int start, int pageSize, long total, IEnumerable<TEntity> data)
        {
            Start = start;
            PageSize = pageSize;
            Total = total;
            Data = data;
        }
    }
}