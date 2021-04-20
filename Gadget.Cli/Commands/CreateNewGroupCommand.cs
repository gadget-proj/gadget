using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("groups create", Description = "creates new group")]
    public class CreateNewGroupCommand : ICommand
    {
        public record CreateNewGroup(string Name);

        [CommandParameter(0, Description = "TODO")]
        public string Value { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var httpClient = new HttpClient();
            var request = new CreateNewGroup(Value);
            var response = await httpClient.PostAsJsonAsync("http://localhost:5001/groups", request);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync(":(");
                return;
            }

            await console.Output.WriteLineAsync(":)");
        }
    }
}