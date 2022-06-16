using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers
{
    public interface IServerHealthCheckWorker: IDisposable
    {
        Task Start(CancellationToken cancellationToken = default);
        Task Stop(CancellationToken cancellationToken = default);
    }
}
