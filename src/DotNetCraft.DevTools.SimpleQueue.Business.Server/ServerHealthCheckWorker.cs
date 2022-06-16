using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs;
using DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers;
using DotNetCraft.DevTools.SimpleQueue.Core.Entities;
using DotNetCraft.DevTools.SimpleQueue.Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Timer = System.Timers.Timer;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server
{
    public class ServerHealthCheckWorker: IServerHealthCheckWorker
    {
        private readonly HealthCheckConfig _healthCheckConfig;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ILogger<ServerHealthCheckWorker> _logger;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private Timer _timer;
        private ServerInfo _currentServerInfo;
        private bool _isUpdating;
        private DateTime? _invalidServerExpiration;

        public ServerHealthCheckWorker(IOptions<HealthCheckConfig> configOptions, IRepositoryFactory repositoryFactory, ILogger<ServerHealthCheckWorker> logger)
        {
            if (configOptions == null || configOptions.Value == null) 
                throw new ArgumentNullException(nameof(configOptions));

            _healthCheckConfig = configOptions.Value;
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            var utc = DateTime.UtcNow;
            var heartbeatTime = utc.AddSeconds(_healthCheckConfig.ThresholdCheck * _healthCheckConfig.HeartBeatSec);
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
                    await serverInfoRepository.InsertAsync(_currentServerInfo, _cts.Token);
                }
                else
                {
                    _currentServerInfo.ServerExpiredTime = heartbeatTime;
                    await serverInfoRepository.UpdateAsync(_currentServerInfo, _cts.Token);
                }

                await serverInfoRepository.DeleteOldServersAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update server info: {ex.Message}");

                if (_invalidServerExpiration.HasValue == false)
                {
                    _invalidServerExpiration = heartbeatTime;
                }

                if (_invalidServerExpiration.Value < utc && _currentServerInfo != null)
                {
                    _logger.LogError("Unable to update server information => server will be deleted");
                    await DeleteServerInfo(_cts.Token);
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private async Task DeleteServerInfo(CancellationToken cancellationToken)
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

            _invalidServerExpiration = null;
            _currentServerInfo = null;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            if (_timer != null)
                throw new Exception("Server health check worker has already been started");

            _timer = new Timer(_healthCheckConfig.HeartBeatSec*1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public async Task Stop(CancellationToken cancellationToken = default)
        {
            if (_timer == null)
                return;

            _timer.Elapsed -= OnTimerElapsed;
            _timer.Stop();
            _cts.Cancel();

            if (_currentServerInfo != null)
            {
                await DeleteServerInfo(cancellationToken);
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                Stop().GetAwaiter().GetResult();
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
