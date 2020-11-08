using Gadget.Messaging.ServiceMessages;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    public class MachineHealthWatcher : BackgroundService
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly ChannelWriter<MachineHealthData> _healthChannelWriter;

        public MachineHealthWatcher(
            PerformanceCounter cpuCounter, 
            ChannelWriter<MachineHealthData> healthChannelWriter)
        {
            _cpuCounter = cpuCounter;
            _healthChannelWriter = healthChannelWriter;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = CheckMachineHealth();
                _healthChannelWriter.WriteAsync(data);
                Task.Delay(10 * 1000);
            }
            return Task.CompletedTask;
        }

        private MachineHealthData CheckMachineHealth()
        {
            var output = new MachineHealthData
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

        private void AddDrivesInfo(MachineHealthData model)
        {
            var discs = DriveInfo.GetDrives();
            model.Discs = discs.Select(x => new DiscUsageInfo
            {
                Name = x.Name,
                DiscSize = x.TotalSize / 1073741824f,
                DiscSpaceFree = x.AvailableFreeSpace / 1073741824f
            }).ToList();
        }

        private void AddMemoryInfo(MachineHealthData model)
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
