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
    [Command("events list")]
    public class GetEventsCommand : Command, ICommand
    {
        public record GetEventsResponse(string Agent, string Service, DateTime CreatedAt, string Status);

        [CommandParameter(0, Description = "agent")]
        public string Agent { get; set; }

        [CommandParameter(1, Description = "service")]
        public string Service { get; set; }


        public async ValueTask ExecuteAsync(IConsole console)
        {
            var response =
                await HttpClient.GetFromJsonAsync<IEnumerable<GetEventsResponse>>($"agents/{Agent}/{Service}/events");
            
            if (response is null)
            {
                await console.Output.WriteLineAsync("bad res");
                return;
            }

            foreach (var getEventsResponse in response)
            {
                await console.Output.WriteLineAsync(getEventsResponse.ToString());
            }
        }

        public GetEventsCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}