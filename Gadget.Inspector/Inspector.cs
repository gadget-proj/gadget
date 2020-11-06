using Gadget.Inspector.Services.Interfaces;
using Gadget.Messaging.RegistrationMessages;
using Gadget.Messaging.ServiceMessages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    public class Inspector
    {
        private readonly ILogger<Inspector> _logger;
        private readonly HubConnection _hubConnection;
        private readonly Guid _id;
        private readonly IDictionary<string, WindowsService> _services;

        public Inspector(
            Uri hubAddress,
            ILogger<Inspector> logger = null)
        {
            _logger ??= logger;
            _id = Guid.NewGuid();
            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(hubAddress)
                .Build();
            _services = new Dictionary<string, WindowsService>();
        }

        public async Task Start()
        {
            await Connect();

            var services = ServiceController
                .GetServices()
                .Select(s => (s.ServiceName, new WindowsService(s)))
                .ToList();

            foreach (var (serviceName, windowsService) in services)
            {
                windowsService.StatusChanged += async (caller, @event) =>
                {
                    await _hubConnection.InvokeAsync("ServiceStatusChanged", new ServiceStatusChanged
                    {
                        AgentId = _id,
                        Name = @event.ServiceName,
                        Status = @event.Status.ToString()
                    });
                };
                _services.Add(serviceName, windowsService);
            }

            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                _logger?.LogError($"{e.Message}");
            }

            var registerNewAgent = new RegisterNewAgent
            {
                AgentId = _id,
                Machine = Environment.MachineName,
                Services = services.Select(s => new Service
                {
                    Name = s.ServiceName,
                    Status = s.Item2?.Status.ToString()
                })
            };
            await _hubConnection.InvokeAsync("Register", registerNewAgent);
             
        }

        private async Task Connect()
        {
            await _hubConnection.StartAsync();
            RegisterHandlers();
            while (_hubConnection.State != HubConnectionState.Connected)
            {
                Console.WriteLine("trying to connect");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task AggregateServices()
        {

        }

        private void RegisterHandlers()
        {
            _hubConnection.On<StopService>("StopService", (command) =>
            {
                Console.WriteLine($"Trying to stop {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service))
                {
                    service.Stop();
                }
            });
            _hubConnection.On<StartService>("StartService", (command) =>
            {
                Console.WriteLine($"Trying to start {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service))
                {
                    service.Start();
                }
            });
        }
    }
}