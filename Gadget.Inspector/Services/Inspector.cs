using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging;
using Gadget.Messaging.Commands;
using Gadget.Messaging.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class Inspector : BackgroundService
    {
        private readonly Channel<ServiceStatusChanged> _channel;
        private readonly HubConnection _hubConnection;
        private readonly Guid _id;
        private readonly ILogger<Inspector> _logger;
        private readonly IDictionary<string, WindowsService> _services;

        public Inspector(Uri hubAddress, Channel<ServiceStatusChanged> channel, ILogger<Inspector> logger = null)
        {
            _channel = channel;
            _logger ??= logger;
            _id = Guid.NewGuid();
            _services = ServiceController
                .GetServices()
                .Select(s => (s.ServiceName, new WindowsService(s, _channel.Writer)))
                .ToDictionary(k => k.ServiceName, v => v.Item2);
            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(hubAddress)
                .Build();
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
            await WatchServices(stoppingToken);
            // stoppingToken.WaitHandle.WaitOne();
        }

        private async Task WatchServices(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
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
                await _hubConnection.InvokeAsync("Register", registerNewAgent, stoppingToken);
                _logger.LogInformation("Registering this agent");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await _channel.Reader.WaitToReadAsync(stoppingToken);
                        var @event = await _channel.Reader.ReadAsync(stoppingToken);
                        @event.AgentId = _id;
                        _logger.LogInformation($"Service {@event.Name} status has changed to {@event.Status}");
                        await _hubConnection.InvokeAsync("ServiceStatusChanged", @event, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }, stoppingToken);
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