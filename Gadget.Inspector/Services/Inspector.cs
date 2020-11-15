using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Inspector.Transport;
using Gadget.Messaging;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class Inspector : BackgroundService
    {
        private readonly Channel<ServiceStatusChanged> _channel;
        private readonly ILogger<Inspector> _logger;
        private readonly IControlPlane _controlPlane;
        private readonly IDictionary<string, ServiceControllerStatus> _statuses;

        public Inspector(Channel<ServiceStatusChanged> channel, IControlPlane controlPlane, ILogger<Inspector> logger)
        {
            _statuses = new Dictionary<string, ServiceControllerStatus>();
            _channel = channel;
            _controlPlane = controlPlane;
            _logger = logger;
        }

        private void RegisterHandlers()
        {
            _controlPlane.RegisterHandler<StopService>("StopService", command =>
            {
                _logger.LogInformation($"Trying to stop {command.ServiceName} service");
                var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == command.ServiceName);
                service?.Stop();
            });
            _controlPlane.RegisterHandler<StartService>("StartService", command =>
            {
                _logger.LogInformation($"Trying to start {command.ServiceName} service");
                var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == command.ServiceName);
                service?.Start();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RegisterAgent();
            _logger.LogInformation($"Starting Watcher ${DateTime.UtcNow}");
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var serviceController in ServiceController.GetServices())
                {
                    if (!_statuses.ContainsKey(serviceController.ServiceName))
                    {
                        _statuses[serviceController.ServiceName] = serviceController.Status;
                        await UpdateStatus(serviceController, stoppingToken);
                    }

                    var previousStatus = _statuses[serviceController.ServiceName];
                    var currentStatus = serviceController.Status;
                    if (currentStatus == previousStatus)
                    {
                        continue;
                    }

                    await UpdateStatus(serviceController, stoppingToken);
                    _statuses[serviceController.ServiceName] = currentStatus;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task RegisterAgent()
        {
            RegisterHandlers();

            var registerNewAgent = new RegisterNewAgent
            {
                Agent = Environment.MachineName.Replace("-", ""),
                Services = ServiceController.GetServices().Select(s => new Service
                {
                    Name = s.ServiceName,
                    Status = s.Status.ToString()
                })
            };
            await _controlPlane.Invoke("Register", registerNewAgent);
        }

        private async Task UpdateStatus(ServiceController serviceController, CancellationToken stoppingToken)
        {
            var @event = new ServiceStatusChanged
            {
                Name = serviceController.ServiceName,
                Status = serviceController.Status.ToString(),
                AgentId = Environment.MachineName.Replace("-", "")
            };
            await _controlPlane.Invoke("ServiceStatusChanged", @event);
        }
    }
}