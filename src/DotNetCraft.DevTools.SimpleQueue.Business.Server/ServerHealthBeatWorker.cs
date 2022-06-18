using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DotNetCraft.DevTools.Abstraction;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs;
using DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers;
using DotNetCraft.DevTools.SimpleQueue.Core.Entities;
using DotNetCraft.DevTools.SimpleQueue.Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server
{
    public class ServerHealthBeatWorker: BaseTimerClass, IServerHealthBeatWorker
    {
        private readonly HealthCheckConfig _healthCheckConfig;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ILogger<ServerHealthBeatWorker> _logger;

        private ServerInfo _currentServerInfo;

        public bool IsAlive { get; private set; } = false;

        public ServerHealthBeatWorker(IOptions<HealthCheckConfig> configOptions, IRepositoryFactory repositoryFactory, ILogger<ServerHealthBeatWorker> logger):
            base(configOptions.Value, logger)
        {
            if (configOptions == null || configOptions.Value == null) 
                throw new ArgumentNullException(nameof(configOptions));

            _healthCheckConfig = configOptions.Value;
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task OnTimerElapsedAsync(object sender, ElapsedEventArgs elapsedEventArgs, CancellationToken ctsToken)
        {
            var utc = DateTime.UtcNow;
            var heartbeatTime = utc.AddMilliseconds(_healthCheckConfig.ThresholdCheck * _healthCheckConfig.IntervalMs);
            var serverInfoRepository = _repositoryFactory.CreateRepository<IServerInfoRepository>();
            try
            {
                if (_currentServerInfo == null)
                {
                    _currentServerInfo = new ServerInfo
                    {
                        Name = _healthCheckConfig.Name,
                        Description = _healthCheckConfig.Description,
                        ServerExpiredTime = heartbeatTime
                    };
                    await serverInfoRepository.InsertAsync(_currentServerInfo, ctsToken);
                }
                else
                {
                    _currentServerInfo.ServerExpiredTime = heartbeatTime;
                    await serverInfoRepository.UpdateAsync(_currentServerInfo, ctsToken);
                }

                IsAlive = true;
            }
            catch (Exception ex)
            {
                _currentServerInfo = null;
                IsAlive = false;
                _logger.LogError(ex, $"Failed to update server info: {ex.Message}");
                throw;
            }
        }

        private async Task DeleteServerInfo(CancellationToken cancellationToken = default)
        {
            try
            {
                var serverInfoRepository = _repositoryFactory.CreateRepository<IServerInfoRepository>();
                await serverInfoRepository.DeleteAsync(_currentServerInfo.Id, cancellationToken);
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, $"Failed to delete server: {ex2.Message}");
            }

            _currentServerInfo = null;
        }

        protected override async Task OnTimerStopAsync()
        {
            await base.OnTimerStopAsync();

            try
            {
                var serverInfoRepository = _repositoryFactory.CreateRepository<IServerInfoRepository>();
                await serverInfoRepository.DeleteAsync(_currentServerInfo.Id);
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, $"Failed to delete server: {ex2.Message}");
            }

            _currentServerInfo = null;
        }
    }
}
