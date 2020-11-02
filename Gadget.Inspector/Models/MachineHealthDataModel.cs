using System.Collections.Generic;

namespace Gadget.Inspector.Models
{
    public class MachineHealthDataModel
    {
        public string MachineName { get; set; } // jest

        public int CpuPercentUsage { get; set; }// jest

        public int RamAvailable { get; set; }// jest

        public int ProcessesQuantity { get; set; }// jest

        public int CpuThreadsQuantity { get; set; } // jest

        public int RamSize { get; set; } // mogę mieć ale tylko na maszynach windowsowych

        public string Platform { get; set; } // jest

        public List<DiscUsageInfo> Discs { get; set; } // jest
    }

    public class DiscUsageInfo
    {
        public string Name { get; set; }

        public int DiscSpaceFree { get; set; }

        public int DiscSize { get; set; }
    }
}
