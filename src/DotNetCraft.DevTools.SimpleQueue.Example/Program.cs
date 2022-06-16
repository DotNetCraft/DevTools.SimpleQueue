using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCraft.DevTools.Repositories.Abstraction;
using DotNetCraft.DevTools.Repositories.Abstraction.Interfaces;
using DotNetCraft.DevTools.SimpleQueue.Business.Server;
using DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs;
using DotNetCraft.DevTools.SimpleQueue.Core.Business.Servers;
using DotNetCraft.DevTools.SimpleQueue.Core.Repositories;
using DotNetCraft.DevTools.SimpleQueue.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCraft.DevTools.SimpleQueue.Example
{
    internal class Program
    {
        static readonly CancellationTokenSource _mainCts = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            var host = CreateDefaultBuilder().Build();

            var sp = host.Services;

            using (var serverHealthWorker = sp.GetService<IServerHealthCheckWorker>())
            {
                await serverHealthWorker.Start(_mainCts.Token);

                await host.RunAsync();
            }
        }

        static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(RegisterServices);
        }

        private static void RegisterServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            var configuration = hostBuilderContext.Configuration;
            var sqlConnection = configuration.GetConnectionString("SimpleQueue");


            var healthCheckConfig = configuration.GetSection(nameof(HealthCheckConfig));
            services.Configure<HealthCheckConfig>(healthCheckConfig);

            services.AddLogging();


            //DbContext should be transient
            //Repository should be scoped
            #region To Common

            services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
            services.AddDbContext<SimpleQueueContext>(options =>
            {
                options
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(sqlConnection);
            }, ServiceLifetime.Transient);

            #endregion

            services.AddScoped<IServerInfoRepository, ServerInfoRepository>();
            services.AddScoped<IServerHealthCheckWorker, ServerHealthCheckWorker>();
        }
    }
}
