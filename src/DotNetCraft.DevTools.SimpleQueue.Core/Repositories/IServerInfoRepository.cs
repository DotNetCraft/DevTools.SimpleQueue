using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.SimpleQueue.Core.Entities;

namespace DotNetCraft.DevTools.SimpleQueue.Core.Repositories
{
    public interface IServerInfoRepository: ISimpleQueueRepository<ServerInfo>
    {
        Task DeleteOldServersAsync(CancellationToken cancellationToken = default);
    }
}
