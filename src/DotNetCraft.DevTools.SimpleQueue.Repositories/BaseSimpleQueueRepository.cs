using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Sql;
using Microsoft.Extensions.Logging;

namespace DotNetCraft.DevTools.SimpleQueue.Repositories
{
    public abstract class BaseSimpleQueueRepository<TEntity>: GenericRepository<SimpleQueueContext, TEntity, long> 
        where TEntity : class
    {
        protected BaseSimpleQueueRepository(SimpleQueueContext dbContext, ILogger<BaseRepository<TEntity, long>> logger) : base(dbContext, logger)
        {
        }
    }
}
