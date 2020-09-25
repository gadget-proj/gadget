using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
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

        public Inspector(Uri hubAddress, ILogger<Inspector> logger = null)
        {
            _logger ??= logger;
            _id = Guid.NewGuid();
            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(hubAddress)
                .Build();
        }

        public async Task Start()
        {
            RegisterHandlers();
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
                Machine = Environment.MachineName
            };
            await _hubConnection.InvokeAsync("Register", registerNewAgent);
        }

        private static ServiceController GetService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(serviceName));
            }
            return ServiceController.GetServices().Single(s => string.Equals(s.ServiceName, serviceName, StringComparison.CurrentCultureIgnoreCase));
        }
        private void RegisterHandlers()
        {
            _hubConnection.On<StopService>("StopService", async (command) =>
            {
                Console.WriteLine($"Trying to stop {command.ServiceName} service");
                var service = GetService(command.ServiceName);
                service.Stop();
                service.Refresh();
                Console.WriteLine(service.Status);
            });
            _hubConnection.On<StartService>("StartService", async (command) =>
            {
                Console.WriteLine($"Trying to start {command.ServiceName} service");
                var service = GetService(command.ServiceName);
                service.Start();
                service.Refresh();
                Console.WriteLine(service.Status);
            });
            _hubConnection.On("GetServicesReport", async () =>
            {
                _logger?.LogInformation($"Received request for services report");
                try
                {
                    var services = ServiceController.GetServices().Select(s => new Messaging.Service
                    {
                        Name = s.ServiceName,
                        Status = s.Status.ToString()
                    }).ToList();

                    _logger?.LogInformation($"{services.Count} service found");

                    var report = new RegisterNewAgent
                    {
                        Machine = Environment.MachineName,
                        AgentId = _id,
                        Services = services
                    };
                    _logger?.LogInformation($"Sending service report");
                    await _hubConnection.InvokeAsync<RegisterMachineReport>("RegisterMachineReport", report);
                }
                catch (Exception e)
                {
                    _logger?.LogError($"{e.Message}");
                }
            });
        }
    }
}