using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Expertec.Sigeco.AuthServer.API.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {        

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}