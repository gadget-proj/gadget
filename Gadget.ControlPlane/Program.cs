using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.ServiceProcess;
using System.Threading.Tasks;
using static System.String;

namespace Gadget.ControlPlane
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("https://localhost:5001/gadget")
                .Build();
            await connection.StartAsync();
            
            Console.Write("agent >");
            var agent = Console.ReadLine();
            Console.Write("svc >");
            var service = Console.ReadLine();
            var command = new StopService
            {
                AgentId = Guid.Parse(agent),
                ServiceName = service
            };
            await connection.InvokeAsync("StopService", command);
            Console.WriteLine("donzo");
            // var service = ServiceController.GetServices().Single(s =>
            //     string.Equals(s.ServiceName, "BTAGService", StringComparison.CurrentCultureIgnoreCase));
            // service.Stop();
            // service.Refresh();
            // Console.WriteLine(service.Status);
        }
    }
}