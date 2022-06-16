using System;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs
{
    public class HealthCheckConfig
    {
        public string Name { get; set; } = Environment.MachineName;
        public string Description { get; set; }
        public int HeartBeatSec { get; set; } = 15;
        public int ThresholdCheck { get; set; } = 3;
    }
}
