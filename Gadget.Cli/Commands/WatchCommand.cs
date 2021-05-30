using System;
using System.Net.Http;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Gadget.Messaging.SignalR.v1;
using Microsoft.AspNetCore.SignalR.Client;

namespace Gadget.Cli.Commands
{
    [Command("watch")]
    public class WatchServiceCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "resource to watch")]
        public string Resource { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/gadget").Build();
            connection.On<ServiceDescriptor>("ServiceStatusChanged",
                async msg =>
                    await console.Output.WriteLineAsync(
                        $"Service [{msg.Name}] running on an agent [{msg.Agent}] changed its status to [{msg.Status}]"));
            await connection.StartAsync();
            await connection.InvokeAsync<string>("Subscribe", Resource);
            console.Input.Read();
        }

        public WatchServiceCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}