using System.Net.Http;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("services stop", Description = "Attempts to stop requested service")]
    public class StopServiceCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "agent name")]
        public string Agent { get; set; }

        [CommandParameter(1, Description = "service name")]
        public string Service { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var response = await HttpClient.PostAsync($"http://localhost:5001/agents/{Agent}/{Service}/stop", null!);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync("could not execute this command successfully");
                return;
            }

            await console.Output.WriteLineAsync("command executed successfully");
        }

        public StopServiceCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}