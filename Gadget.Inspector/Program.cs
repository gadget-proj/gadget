using System;
using System.Linq;
using System.ServiceProcess;
using System.Text.Json;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.Inspector
{
    class Program
    {
        private static async Task Main()
        {
            var id = Guid.NewGuid();
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("http://localhost:5000/gadget")
                .Build();
            await connection.StartAsync();
            var registerNewAgent = new RegisterNewAgent
            {
                AgentId = id,
                Machine = Environment.MachineName
            };
            await connection.InvokeAsync("Register", registerNewAgent);
            connection.On("GetServicesReport", async () =>
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
                        AgentId = id,
                        Services = services
                    };
                    await connection.InvokeAsync<RegisterMachineReport>("RegisterMachineReport", report);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            Console.WriteLine("Registered");
            Console.ReadKey();
        }
    }
}