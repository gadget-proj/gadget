using System;
using System.Threading.Tasks;
using Gadget.Messaging.SignalR;
using Gadget.Messaging.SignalR.v1;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.ConsoleApp.Debug
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/notifications").Build();
            connection.On<ServiceDescriptor>("ServiceStatusChanged",
                msg => Console.WriteLine($"{msg.Name} {msg.Name} {msg.Status}"));
            await connection.StartAsync();
            Console.WriteLine(connection.State);
            Console.ReadKey();
        }
    }
}