using System;
using System.Collections.Generic;

namespace Gadget.Messaging.ServiceMessages
{
    public class MachineHealthData
    {
        public Guid MachineId { get; set; }

        public string MachineName { get; set; }

        public int CpuPercentUsage { get; set; }

        public int ProcessesQuantity { get; set; }

        public int CpuThreadsQuantity { get; set; }

        public float MemoryFree { get; set; }
        public float MemoryTotal { get; set; }

        public string Platform { get; set; }

        public List<DiscUsageInfo> Discs { get; set; }
    }
}
