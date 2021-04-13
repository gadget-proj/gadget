using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("services list", Description = "lists all services registered on an agent")]
    public class GetAgentServicesCommand : ICommand
    {
        [CommandParameter(0, Description = "ok")]
        public string Value { get; set; }

        public record GetServicesResponse(string Name, string Description, string LogOnAs, string Status);

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var client = new HttpClient();
            var services =
                await client.GetFromJsonAsync<IEnumerable<GetServicesResponse>>(
                    $"http://localhost:5001/agents/{Value}");
            if (services is null)
            {
                await console.Output.WriteLineAsync("something went wrong, response is null");
                return;
            }

            var servicesList = services.ToList();
            if (!servicesList.Any())
            {
                await console.Output.WriteLineAsync($"could not find any registered services for this agent ({Value})");
                return;
            }

            foreach (var getServicesResponse in servicesList)
            {
                console.WithForegroundColor(ConsoleColor.Green);
                await console.Output.WriteAsync(">");

                console.WithForegroundColor(ConsoleColor.Yellow);
                await console.Output.WriteAsync("[");
                console.WithForegroundColor(ConsoleColor.White);
                await console.Output.WriteAsync($"{getServicesResponse.Name}");
                console.WithForegroundColor(ConsoleColor.Yellow);
                await console.Output.WriteAsync("]");
                console.WithForegroundColor(getServicesResponse.Status == "Stopped"
                    ? ConsoleColor.Red
                    : ConsoleColor.Green);
                await console.Output.WriteLineAsync($" {getServicesResponse.Status}");
            }
        }
    }
}