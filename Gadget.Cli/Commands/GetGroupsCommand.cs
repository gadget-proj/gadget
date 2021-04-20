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
    [Command("groups list", Description = "lists all groups")]
    public class GetGroupsCommand : ICommand
    {
        public record GetGroupsRequest(Guid Id, string Name);

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var client = new HttpClient();
            // var response = await client.GetFromJsonAsync<IEnumerable<GetGroupsRequest>>("http://localhost:5001/groups");
            var response = await client.GetFromJsonAsync<IEnumerable<GetGroupsRequest>>("http://nmv10:5001/groups");
            if (response is null)
            {
                await console.Output.WriteLineAsync(":(");
                return;
            }

            foreach (var (guid, name) in response)
            {
                await console.Output.WriteLineAsync($"Id : {guid} Name : {name}");
            }
        }
    }
}