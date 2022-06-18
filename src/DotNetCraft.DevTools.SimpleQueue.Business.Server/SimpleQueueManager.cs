using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DotNetCraft.DevTools.Abstraction;
using DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs;
using DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server
{
    public class SimpleQueueManager: BaseTimerClass, ISimpleQueueManager
    {
        private readonly IServerHealthBeatWorker _healthBeatWorker;
        private readonly ILogger<SimpleQueueManager> _logger;
        private readonly SimpleQueueManagerConfig _config;
        
        public SimpleQueueManager(IOptions<SimpleQueueManagerConfig> options, IServerHealthBeatWorker healthBeatWorker, ILogger<SimpleQueueManager> logger): base(options.Value, logger)
        {
            _healthBeatWorker = healthBeatWorker ?? throw new ArgumentNullException(nameof(healthBeatWorker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = options?.Value ?? throw new ArgumentNullException(nameof(options));

        }

        protected override async Task OnTimerElapsedAsync(object sender, ElapsedEventArgs elapsedEventArgs, CancellationToken ctsToken)
        {
            if (_healthBeatWorker.IsAlive)
            {
                //await _taskEngine.ExecuteAsync(cancellationToken);
            }
        }

        protected override async Task OnTimerStartAsync()
        {
            _logger.LogInformation($"Starting {_config.Name}...");
            try
            {
                await _healthBeatWorker.StartAsync(CancellationToken.None);
                //await _expiredServerRemover.StartAsync();
                //await _taskEngine.StartAsync();

                _logger.LogInformation($"{_config.Name} was started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start {_config.Name}: {ex.Message}");
                throw;
            }

            await base.OnTimerStartAsync();
        }

        protected override async Task OnTimerStopAsync()
        {
            await base.OnTimerStopAsync();

            _logger.LogInformation($"Stopping {_config.Name}...");
            try
            {
                await _healthBeatWorker.StopAsync(CancellationToken.None);
                //await _expiredServerRemover.StopAsync();
                //await _taskEngine.StopAsync();

                _logger.LogInformation($"{_config.Name} was started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to stop {_config.Name}: {ex.Message}");
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _logger.LogInformation($"Disposing {_config.Name}...");
                try
                {
                    _healthBeatWorker.Dispose();
                    //_expiredServerRemover.Dispose();
                    //_taskEngine.Dispose();

                    _logger.LogInformation($"{_config.Name} was disposed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to dispose {_config.Name}: {ex.Message}");
                }
            }
        }
    }
}
