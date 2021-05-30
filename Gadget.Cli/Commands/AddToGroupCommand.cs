using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("groups add", Description = "add selected services to a group")]
    public class AddToGroupCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "group to add to")]
        public string GroupName { get; set; }

        [CommandParameter(1, Description = "resource to be added")]
        public string ResourceName { get; set; }

        public record AddToGroupRequest(string ResourceName);

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var request = new AddToGroupRequest(ResourceName);
            var response = await HttpClient.PostAsJsonAsync($"http://localhost:5001/groups/{GroupName}", request);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync(":(");
                return;
            }

            await console.Output.WriteLineAsync(":)");
        }

        public AddToGroupCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}