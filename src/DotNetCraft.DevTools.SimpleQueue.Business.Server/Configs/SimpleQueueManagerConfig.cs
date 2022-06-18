using DotNetCraft.DevTools.Abstraction;

namespace DotNetCraft.DevTools.SimpleQueue.Business.Server.Configs
{
    public class SimpleQueueManagerConfig: ITimerConfig
    {
        public string Name { get; set; } = "Simple Queue Server";
        public double IntervalMs { get; set; } = 15 * 1000;

        public override string ToString()
        {
            return $"{Name}: IntervalMs[{IntervalMs}]";
        }
    }
}
