using System;
using System.Collections.Generic;

namespace DotNetCraft.DevTools.SimpleQueue.Core.Entities
{
    public class ServerInfo: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ServerExpiredTime { get; set; }
    }
}
