using Gadget.Inspector.Models;
using Gadget.Inspector.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Gadget.Inspector.Services
{
    internal class MachineHealthService : IMachineHealthService
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;

        public MachineHealthService()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public MachineHealthDataModel CheckMachineHealth()
        {
            var output =  new MachineHealthDataModel
            {
                MachineName = Environment.MachineName,
                CpuPercentUsage = (int)_cpuCounter.NextValue(),
                RamAvailable = (int)_ramCounter.NextValue(),
                ProcessesQuantity = Process.GetProcesses().Length,
                Platform = Environment.OSVersion.Platform.ToString(),
                CpuThreadsQuantity = Environment.ProcessorCount
            };

            AddDrivesInfo(output);
            return output;
        }

        private void AddDrivesInfo(MachineHealthDataModel model)
        {
            var discs = DriveInfo.GetDrives();
            model.Discs = new List<DiscUsageInfo>();

            foreach (var disc in discs)
            {
                model.Discs.Add(new DiscUsageInfo
                {
                    Name = disc.Name,
                    DiscSize = (int)(disc.TotalSize / 1073741824),
                    DiscSpaceFree = (int)(disc.AvailableFreeSpace / 1073741824)
                });
            }
        }

    }
}
