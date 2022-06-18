using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers;
using Microsoft.Extensions.Hosting;

namespace DotNetCraft.DevTools.SimpleQueue.Example
{
    public class HostApplicationLifetimeEventsHostedService : IHostedService
    {
        private readonly IServerHealthBeatWorker _serverHealthBeatWorker;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public HostApplicationLifetimeEventsHostedService(IServerHealthBeatWorker serverHealthBeatWorker, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serverHealthBeatWorker = serverHealthBeatWorker ?? throw new ArgumentNullException(nameof(serverHealthBeatWorker));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

            await _serverHealthBeatWorker.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serverHealthBeatWorker.StopAsync(cancellationToken);
            //return Task.CompletedTask;
        }

        private void OnStarted()
        {
            // ...
        }

        private async void OnStopping()
        {
            // ...
        }

        private void OnStopped()
        {
            // ...
        }
    }
}
