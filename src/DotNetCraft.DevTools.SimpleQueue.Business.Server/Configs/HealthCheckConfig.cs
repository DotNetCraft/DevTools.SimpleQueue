using System;
using DotNetCraft.DevTools.Abstraction;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs
{
    public class HealthCheckConfig: ITimerConfig
    {
        public string Name { get; set; } = Environment.MachineName;
        public string Description { get; set; }
        public int ThresholdCheck { get; set; } = 3;
        public double IntervalMs { get; set; } = 15 * 1000;
    }
}
