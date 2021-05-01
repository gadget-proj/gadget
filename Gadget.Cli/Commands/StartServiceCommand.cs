using System.Net.Http;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("services start", Description = "Attempts to start requested service")]
    public class StartServiceCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "agent name")]
        public string Agent { get; set; }

        [CommandParameter(1, Description = "service name")]
        public string Service { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var response = await HttpClient.PostAsync($"agents/{Agent}/{Service}/start", null!);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync("bad");
                return;
            }

            await console.Output.WriteLineAsync("good");
        }

        public StartServiceCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}