﻿using Gadget.Messaging;
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

        public Inspector(Uri hubAddress, ILogger<Inspector> logger = null)
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
            RegisterHandlers();
            var services = ServiceController.GetServices().Select(s => (s.ServiceName, new WindowsService(s)));
            foreach (var s in services)
            {
                s.Item2.StatusChanged += (caller, @event) =>
                {
                    Console.WriteLine($"[{@event.ServiceName}] status has changed");
                };
                _services.Add(s.ServiceName, s.Item2);
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
                Services = services.Select(s => new Messaging.Service
                {
                    Name = s.ServiceName,
                    Status = s.Item2.Status.ToString()
                })
            };
            await _hubConnection.InvokeAsync("Register", registerNewAgent);
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<StopService>("StopService", async (command) =>
            {
                Console.WriteLine($"Trying to stop {command.ServiceName} service");
                if (_services.TryGetValue(command.ServiceName, out var service))
                {
                    service.Stop();
                }
            });
            _hubConnection.On<StartService>("StartService", async (command) =>
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