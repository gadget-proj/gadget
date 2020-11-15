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

        public Inspector(Channel<ServiceStatusChanged> channel, IControlPlane controlPlane, ILogger<Inspector> logger)
        {
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _channel.Reader.WaitToReadAsync(stoppingToken);
                    var @event = await _channel.Reader.ReadAsync(stoppingToken);
                    _logger.LogInformation($"Service {@event.Name} status has changed to {@event.Status}");
                    await _controlPlane.Invoke("ServiceStatusChanged", @event);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }

            async Task RegisterAgent()
            {
                RegisterHandlers();

                var registerNewAgent = new RegisterNewAgent
                {
                    Agent = Environment.MachineName.Replace("-",""),
                    Services = ServiceController.GetServices().Select(s => new Service
                    {
                        Name = s.ServiceName,
                        Status = s.Status.ToString()
                    })
                };
                await _controlPlane.Invoke("Register", registerNewAgent);
            }
        }
    }
}