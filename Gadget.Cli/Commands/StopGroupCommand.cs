using System.Net.Http;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("groups stop", Description = "Attempts to stop requested group")]
    public class StopGroupCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "group name")]
        public string GroupName { get; set; }


        public async ValueTask ExecuteAsync(IConsole console)
        {
            var response = await HttpClient.PostAsync($"http://localhost:5001/groups/{GroupName}/stop", null!);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync("bad");
                return;
            }

            await console.Output.WriteLineAsync("good");
        }

        public StopGroupCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}