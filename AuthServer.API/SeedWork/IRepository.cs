namespace Expertec.Sigeco.AuthServer.API.SeedWork
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        
    }
}