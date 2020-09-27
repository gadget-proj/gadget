using System;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.ControlPlane
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                // .WithUrl("https://localhost:5001/gadget")
                // .WithUrl("https://unfold.azurewebsites.net/gadget")
                .WithUrl("https://webscoket.noinputsignal.com/gadget/gadget/")
                .Build();
            await connection.StartAsync();
            await connection.InvokeAsync("RegisterDashboard", new RegisterNewDashboard());
            connection.On<ServiceStatusChanged>("ServiceStatusChanged", msg =>
            {
                Console.WriteLine($"{msg.Name} {msg.Status}");
            });
            Console.WriteLine("ok!");
            Console.ReadLine();
            // while (true)
            // {fxs-draganddrop
            //     var input = Console.ReadLine();
            //     ICommand command;
            //     switch (input)
            //     {
            //         case "list":
            //             command = new GetAllAgents();
            //             break;
            //         default:
            //             continue;
            //     }
            //
            //     await command.Execute();
            // }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}