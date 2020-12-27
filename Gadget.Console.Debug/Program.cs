using System;
using System.Threading.Tasks;
using Gadget.Messaging.Events;
using Gadget.Messaging.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.ConsoleApp.Debug
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder().WithUrl("https://localhost:5001/gadget").Build();
            connection.On<ServiceStatusChanged>("ServiceStatusChanged",
                msg => Console.WriteLine($"{msg.Name} {msg.Name} {msg.Status}"));
            await connection.StartAsync();
            Console.ReadKey();
        }
    }
}