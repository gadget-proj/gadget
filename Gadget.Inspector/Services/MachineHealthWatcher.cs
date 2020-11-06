using Gadget.Inspector.Services.Interfaces;
using Gadget.Messaging.ServiceMessages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Inspector.Services
{
    internal class MachineHealthWatcher : BackgroundService
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly ILogger<MachineHealthWatcher> _logger;

        public MachineHealthWatcher(PerformanceCounter cpuCounter, ILogger<MachineHealthWatcher> logger)
        {
            _cpuCounter = cpuCounter;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = CheckMachineHealth();
               // data.MachineId = _id;

                //await _hubConnection.InvokeAsync("MachineHealthCheck", data);
                //await Task.Delay(10000);
            }
            return Task.CompletedTask;
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
