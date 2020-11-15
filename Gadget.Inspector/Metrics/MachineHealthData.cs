using System;
using System.Collections.Generic;

namespace Gadget.Inspector.Metrics
{
    public class MachineHealthData
    {
        public string Agent { get; set; }
        public string MachineName { get; set; }
        public int CpuPercentUsage { get; set; }
        public int ProcessesQuantity { get; set; }
        public int CpuThreadsQuantity { get; set; }
        public float MemoryFree { get; set; }
        public float MemoryTotal { get; set; }
        public string Platform { get; set; }
        public IEnumerable<DiscUsageInfo> Discs { get; set; } = new List<DiscUsageInfo>();

        public override string ToString()
        {
            return $"AGN : {Agent}" +
                   $"{Environment.NewLine}" +
                   $"CPU : {CpuPercentUsage}" +
                   $"{Environment.NewLine}" +
                   $"MEF : {MemoryFree}" +
                   $"{Environment.NewLine}" +
                   $"MET : {MemoryTotal}";
        }
    }
}