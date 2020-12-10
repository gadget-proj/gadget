using Gadget.Inspector.HandlerRegistration;
using Gadget.Inspector.Metrics;
using Gadget.Inspector.Metrics.Services;
using Gadget.Inspector.Transport;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gadget.Inspector.Services
{
    public class Inspector : BackgroundService
    {
        private readonly ILogger<Inspector> _logger;
        private readonly IControlPlane _controlPlane;
        private readonly RegisterHandlers _registerHandlers;
        private readonly InspectorResources _resources;
        private readonly WindowsServiceCheck _serviceScheck;


        public Inspector(
            ILogger<Inspector> logger, 
            InspectorResources resources,
            IControlPlane controlPlane,
            RegisterHandlers registerHandlers,
            WindowsServiceCheck serviceScheck) 
        {
            _logger = logger;
            _resources = resources;
            _controlPlane = controlPlane;
            _registerHandlers = registerHandlers;
            _serviceScheck = serviceScheck;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RegisterAgent();
            _logger.LogInformation($"Starting Watcher ${DateTime.UtcNow}");
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var statusChanged in _serviceScheck.CheckServices())
                {
                    await UpdateStatus(statusChanged);
                }

                var data =  _resources.GetMachineHealthData();
                //data.Agent = "tmp";
                //await UpdateMachineHealth(data);
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        private async Task RegisterAgent()
        {
            _registerHandlers.Register();
            var registerNewAgent = _serviceScheck.RegisterServices();
            await _controlPlane.Invoke("Register", registerNewAgent);
        }

        private async Task UpdateStatus(ServiceStatusChanged @event)
        {
            await _controlPlane.Invoke("ServiceStatusChanged", @event);
        }

        //private async Task UpdateMachineHealth(GetAgentHealth @event)
        //{
        //    await _controlPlane.Invoke("GetAgentHealth", @event);
        //}
    }
}