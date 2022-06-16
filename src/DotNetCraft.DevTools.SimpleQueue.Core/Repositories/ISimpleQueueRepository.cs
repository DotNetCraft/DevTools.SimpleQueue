using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;

namespace DotNetCraft.DevTools.SimpleQueue.Core.Repositories
{
    public interface ISimpleQueueRepository<TEntity>: IRepository<TEntity, long> 
        where TEntity : class
    {
    }
}
