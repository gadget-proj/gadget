using Gadget.Inspector.Services.Interfaces;
using Gadget.Messaging.ServiceMessages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace Gadget.Inspector.Services
{
    internal class MachineHealthService : IMachineHealthService
    {
        private readonly PerformanceCounter _cpuCounter;

        public MachineHealthService()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
           // _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public MachineHealthDataModel CheckMachineHealth()
        {
            var output =  new MachineHealthDataModel
            {
                MachineName = Environment.MachineName,
                CpuPercentUsage = (int)_cpuCounter.NextValue(),
                ProcessesQuantity = Process.GetProcesses().Length,
                Platform = Environment.OSVersion.Platform.ToString(),
                CpuThreadsQuantity = Environment.ProcessorCount
            };

            AddDrivesInfo(output);
            AddMemoryInfo(output);
            return output;
        }

        private void AddDrivesInfo(MachineHealthDataModel model)
        {
            var discs = DriveInfo.GetDrives();
            model.Discs = discs.Select(x => new DiscUsageInfo
            {
                Name = x.Name,
                DiscSize = x.TotalSize / 1073741824f,
                DiscSpaceFree = x.AvailableFreeSpace / 1073741824f
            }).ToList();
        }

        private void AddMemoryInfo(MachineHealthDataModel model)
        {
            var qo = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            var searcher = new ManagementObjectSearcher(qo);
            var results = searcher.Get();

            var result = results.OfType<ManagementObject>().FirstOrDefault();
            if (result == null) return;

            if (int.TryParse(result["TotalVisibleMemorySize"].ToString(), out int total))
            {
                model.MemoryTotal = total / 1048576f;
            }

            if (int.TryParse(result["FreePhysicalMemory"].ToString(), out int free))
            {
                model.MemoryFree = free / 1048576f;
            }
        }
    }
}
