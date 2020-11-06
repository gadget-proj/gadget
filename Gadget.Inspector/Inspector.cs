using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class Inspector : BackgroundService
    {
        private readonly HubConnection _hubConnection;
        private readonly Guid _id;
        private readonly ILogger<Inspector> _logger;
        private readonly IDictionary<string, WindowsService> _services;
        private readonly Channel<ServiceStatusChanged> _channel;

        public Inspector(Uri hubAddress, Channel<ServiceStatusChanged> channel, ILogger<Inspector> logger = null)
        {
            _channel = channel;
            _logger ??= logger;
            _id = Guid.NewGuid();
            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(hubAddress)
                .Build();
            _services = new Dictionary<string, WindowsService>();
        }


        private void RegisterHandlers()
        {
            _hubConnection.On<StopService>("StopService", command =>
            {
                _logger.LogInformation($"Trying to stop {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service)) service.Stop();
            });
            _hubConnection.On<StartService>("StartService", command =>
            {
                _logger.LogInformation($"Trying to start {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service)) service.Start();
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConnectToHub(stoppingToken);
            var services = RegisterServices(stoppingToken);
            await RegisterInspector(stoppingToken, services);
            stoppingToken.WaitHandle.WaitOne();
        }

        private async Task RegisterInspector(CancellationToken stoppingToken,
            IEnumerable<(string ServiceName, WindowsService)> services)
        {
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
            await _hubConnection.InvokeAsync("Register", registerNewAgent, stoppingToken);
        }

        private IEnumerable<(string ServiceName, WindowsService)> RegisterServices(CancellationToken stoppingToken)
        {
            var services = ServiceController
                .GetServices()
                .Select(s => (s.ServiceName, new WindowsService(s)))
                .ToList();

            foreach (var (serviceName, windowsService) in services)
            {
                windowsService.StatusChanged += async (caller, @event) =>
                {
                    var change = new ServiceStatusChanged
                    {
                        AgentId = _id,
                        Name = @event.ServiceName,
                        Status = @event.Status.ToString()
                    };
                    await _channel.Writer.WriteAsync(change, stoppingToken);
                    // await _hubConnection.InvokeAsync("ServiceStatusChanged", , stoppingToken);
                };
                _services.Add(serviceName, windowsService);
            }

            return services;
        }

        private async Task ConnectToHub(CancellationToken stoppingToken)
        {
            await _hubConnection.StartAsync(stoppingToken);
            RegisterHandlers();
            while (_hubConnection.State != HubConnectionState.Connected)
            {
                _logger.LogInformation("trying to connect");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}