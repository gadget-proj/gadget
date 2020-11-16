using Gadget.Inspector.Metrics;
using Gadget.Inspector.Metrics.Services;
using Gadget.Inspector.Transport;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Inspector.Services
{
    public class Inspector : BackgroundService
    {
        private readonly ILogger<Inspector> _logger;
        private readonly IControlPlane _controlPlane;
        private readonly InspectorResources _resources;
        private readonly WindowsServiceCheck _serviceScheck;


        public Inspector(
            ILogger<Inspector> logger, 
            InspectorResources resources,
            IControlPlane controlPlane,
            WindowsServiceCheck serviceScheck) 
        {
            _logger = logger;
            _resources = resources;
            _controlPlane = controlPlane;
            _serviceScheck = serviceScheck;
        }

        // TODO Possibly replace this method with some kind of reflection trick to register different handler (split each handler into different class) for each signalR method?
        // This could possibly grow really big in the future and become really hard to maintain
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
            _controlPlane.RegisterHandler<GetAgentHealth>("GetServicesReport", _ =>
            {
                _logger.LogInformation("GetServicesReport");
                var report = _resources.CheckMachineHealth();
                _logger.LogInformation($"Healthcheck for agent {Environment.MachineName} => {report}");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RegisterAgent();
            _logger.LogInformation($"Starting Watcher ${DateTime.UtcNow}");
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var statusChanged in _serviceScheck.CheckServices())
                {
                    await UpdateStatus(statusChanged, stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task RegisterAgent()
        {
            RegisterHandlers();
            var registerNewAgent = _serviceScheck.RegisterServices();
            await _controlPlane.Invoke("Register", registerNewAgent);
        }

        private async Task UpdateStatus(ServiceStatusChanged @event, CancellationToken stoppingToken)
        {
            await _controlPlane.Invoke("ServiceStatusChanged", @event);
        }
    }
}