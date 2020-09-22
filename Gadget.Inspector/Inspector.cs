using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.Inspector
{
    public class Inspector
    {
        private readonly HubConnection _hubConnection;
        private readonly Guid _id;

        public Inspector(Uri hubAddress)
        {
            _id = Guid.NewGuid();
            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(hubAddress)
                .Build();
        }

        public async Task Start()
        {
            _hubConnection.On("GetServicesReport", async () =>
            {
                Console.WriteLine("On GetServicesReport");
                try
                {
                    var services = ServiceController.GetServices().Select(s => new Messaging.Service
                    {
                        Name = s.ServiceName,
                        Status = s.Status.ToString()
                    });
                    var report = new RegisterNewAgent
                    {
                        Machine = Environment.MachineName,
                        AgentId = _id,
                        Services = services
                    };
                    await _hubConnection.InvokeAsync<RegisterMachineReport>("RegisterMachineReport", report);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var registerNewAgent = new RegisterNewAgent
            {
                AgentId = _id,
                Machine = Environment.MachineName
            };
            await _hubConnection.InvokeAsync("Register", registerNewAgent);
        }
    }
}