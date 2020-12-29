﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace Gadget.Inspector
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
                CpuPercentUsage = (int)_cpuCounter.NextValue(),
            };

            AddDrivesInfo(output);
            AddMemoryInfo(output);
            return output;
        }

        private static void AddDrivesInfo(MachineHealthData model)
        {
            var discs = DriveInfo.GetDrives();

            foreach (var d in discs)
            {
                model.DiscTotal += (int)(d.TotalSize / 1073741824f);
                model.DiscOccupied += (int)(d.AvailableFreeSpace / 1073741824f);
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