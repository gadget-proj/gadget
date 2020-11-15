using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace Gadget.Inspector.Metrics
{
    public class InspectorResources
    {
        private readonly PerformanceCounter _cpuCounter;

        public InspectorResources(PerformanceCounter cpuCounter)
        {
            _cpuCounter = cpuCounter;
        }
        public MachineHealthData CheckMachineHealth()
        {
            var output = new MachineHealthData
            {
                MachineName = Environment.MachineName,
                CpuPercentUsage = (int) _cpuCounter.NextValue(),
                ProcessesQuantity = Process.GetProcesses().Length,
                Platform = Environment.OSVersion.Platform.ToString(),
                CpuThreadsQuantity = Environment.ProcessorCount
            };

            AddDrivesInfo(output);
            AddMemoryInfo(output);
            return output;
        }

        private static void AddDrivesInfo(MachineHealthData model)
        {
            model.Discs = DriveInfo.GetDrives().Select(Disks);

            static DiscUsageInfo Disks(DriveInfo driveInfo)
            {
                return new DiscUsageInfo
                {
                    Name = driveInfo.Name,
                    DiscSize = driveInfo.TotalSize / 1073741824f,
                    DiscSpaceFree = driveInfo.AvailableFreeSpace / 1073741824f
                };
            }
        }

        private static void AddMemoryInfo(MachineHealthData model)
        {
            var qo = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            var searcher = new ManagementObjectSearcher(qo);
            var results = searcher.Get();
            var result = results.OfType<ManagementObject>().FirstOrDefault();
            if (result is null) return;
            if (int.TryParse(result["TotalVisibleMemorySize"].ToString(), out var total))
            {
                model.MemoryTotal = total / 1048576f;
            }

            if (int.TryParse(result["FreePhysicalMemory"].ToString(), out var free))
            {
                model.MemoryFree = free / 1048576f;
            }
        }
    }
}