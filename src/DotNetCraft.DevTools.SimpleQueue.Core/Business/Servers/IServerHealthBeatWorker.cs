using System;
using DotNetCraft.DevTools.Abstraction;

namespace DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers
{
    public interface IServerHealthBeatWorker: IStartStop, IDisposable
    {
        bool IsAlive { get; }
    }
}
