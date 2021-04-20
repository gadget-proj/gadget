using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("agents list", Description = "lists all registered agents")]
    public class GetAgentsCommand : ICommand
    {
        public record AgentsResponse(string Name, string Address);

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var client = new HttpClient();
            var agents = await client.GetFromJsonAsync<IEnumerable<AgentsResponse>>("http://nmv10:5001/agents");
            // var agents = await client.GetFromJsonAsync<IEnumerable<AgentsResponse>>("http://localhost:5001/agents");
            if (agents is null)
            {
                await console.Output.WriteLineAsync("Could not find any registered agents");
                return;
            }

            foreach (var (name, address) in agents)
            {
                console.WithForegroundColor(ConsoleColor.Green);
                await console.Output.WriteAsync(">");
                
                console.WithForegroundColor(ConsoleColor.Yellow);
                await console.Output.WriteAsync($"[");
                console.WithForegroundColor(ConsoleColor.White);
                await console.Output.WriteAsync($"{name}");
                console.WithForegroundColor(ConsoleColor.Yellow);
                await console.Output.WriteAsync($"]");
                console.WithForegroundColor(ConsoleColor.White);
                await console.Output.WriteLineAsync($" {address}");
                
            }
        }
    }
}