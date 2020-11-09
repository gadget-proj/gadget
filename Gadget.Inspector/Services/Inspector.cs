using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Inspector.Transport;
using Gadget.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class Inspector : BackgroundService
    {
        private readonly Channel<ServiceStatusChanged> _channel;
        private readonly Guid _id;
        private readonly ILogger<Inspector> _logger;
        private readonly IDictionary<string, WindowsService> _services;
        private readonly IControlPlane _controlPlane;

        public Inspector(Channel<ServiceStatusChanged> channel, IControlPlane controlPlane,
            ILogger<Inspector> logger = null)
        {
            _channel = channel;
            _controlPlane = controlPlane;
            _logger ??= logger;
            _id = Guid.NewGuid();
            _services = ServiceController
                .GetServices()
                .Select(s => (s.ServiceName, new WindowsService(s, _channel.Writer)))
                .ToDictionary(k => k.ServiceName, v => v.Item2);
        }


        private void RegisterHandlers()
        {
            _controlPlane.RegisterHandler<StopService>("StopService", command =>
            {
                _logger.LogInformation($"Trying to stop {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service))
                {
                    service.Stop();
                }
            });
            _controlPlane.RegisterHandler<StartService>("StartService", command =>
            {
                _logger.LogInformation($"Trying to start {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service))
                {
                    service.Start();
                }
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Register();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _channel.Reader.WaitToReadAsync(stoppingToken);
                    var @event = await _channel.Reader.ReadAsync(stoppingToken);
                    @event.AgentId = _id;
                    _logger.LogInformation($"Service {@event.Name} status has changed to {@event.Status}");
                    await _controlPlane.Invoke("ServiceStatusChanged", @event);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }

            async Task Register()
            {
                var registerNewAgent = new RegisterNewAgent
                {
                    AgentId = _id,
                    Machine = Environment.MachineName,
                    Services = _services.Select(s => new Service
                    {
                        Name = s.Key,
                        Status = s.Value?.Status.ToString()
                    })
                };
                await _controlPlane.Invoke("Register", registerNewAgent);
                _logger.LogInformation("Registering this agent");
            }
        }
    }
}