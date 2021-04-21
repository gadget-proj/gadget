using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Messaging.SignalR.v1;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class Inspector : BackgroundService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<Inspector> _logger;
        private readonly int _loopInterval;

        private readonly IDictionary<string, ServiceControllerStatus> _services =
            new Dictionary<string, ServiceControllerStatus>();

        public Inspector(IPublishEndpoint publishEndpoint, ILogger<Inspector> logger, IConfiguration configuration)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _loopInterval = int.TryParse(configuration["MainLoopInterval"], out var interval) ? interval : 1;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting watcher {DateTime.UtcNow}");
            await RegisterAgent(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var serviceController in ServiceController.GetServices())
                {
                    await WatchForServiceChanges(stoppingToken, serviceController);
                }

                await Task.Delay(TimeSpan.FromSeconds(_loopInterval), stoppingToken);
            }
        }

        private async Task WatchForServiceChanges(CancellationToken stoppingToken, ServiceController serviceController)
        {
            serviceController.Refresh();
            var current = serviceController.Status;
            if (!_services.TryGetValue(serviceController.ServiceName, out var previous))
            {
                _services[serviceController.ServiceName] = current;
                return;
            }

            if (current == previous)
            {
                return;
            }

            _services[serviceController.ServiceName] = current;
            await _publishEndpoint.Publish<IServiceStatusChanged>(new
            {
                Agent = Environment.MachineName,
                Name = serviceController.ServiceName,
                Status = current.ToString()
            }, stoppingToken);
        }

        private async Task RegisterAgent(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Registering new agent {Environment.MachineName}");
            await _publishEndpoint.Publish<IRegisterNewAgent>(new
            {
                Agent = Environment.MachineName,
                Address = GetAddress(),
                Services = ServiceController.GetServices().Select(s => new ServiceDescriptor
                {
                    Name = s.ServiceName.Trim().ToLower(),
                    Status = s.Status.ToString(),
                    LogOnAs = GetServiceUser(s.ServiceName),
                    Description = GetServiceDescription(s.ServiceName)
                })
            }, stoppingToken);
        }

        private static string GetAddress()
        {
            var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            return addresses.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();
        }

        private static string GetServiceUser(string serviceName)
        {
            var output = "";
            var command = $"Win32_Service.Name='{serviceName}'";
            var wmiService = new ManagementObject(command);
            wmiService.Get();
            try
            {
                output = wmiService["startname"].ToString();
            }
            catch (Exception)
            {
                // ignored
            }

            return output;
        }

        private static string GetServiceDescription(string serviceName)
        {
            var output = "";
            var command = $"Win32_Service.Name='{serviceName}'";
            var wmiService = new ManagementObject(command);
            try
            {
                output = wmiService["Description"].ToString();
            }
            catch (Exception)
            {
                // ignored
            }

            return output;
        }
    }
}