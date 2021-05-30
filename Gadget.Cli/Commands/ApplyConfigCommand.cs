using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    public record ConfigFile(IEnumerable<ConfigRequest> Rules);

    public record ActionRequest (string Event, string Command);

    public record ConfigRequest (string Selector, IEnumerable<ActionRequest> Actions);

    [Command("config apply")]
    public class ApplyConfigCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "json config file")]
        public string Config { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var exists = File.Exists(Config);
            if (!exists)
            {
                await console.Output.WriteLineAsync($"{Config} does not exists");
                return;
            }

            var content = await File.ReadAllTextAsync(Config);

            var valid = JsonSerializer.Deserialize<ConfigFile>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            await console.Output.WriteLineAsync(valid?.ToString());
            await console.Output.WriteLineAsync(valid?.Rules.First().Selector);
            await console.Output.WriteLineAsync(content);

            var response = await HttpClient.PostAsJsonAsync("/resource/config/apply", valid);
            var res = await response.Content.ReadAsStringAsync();
            await console.Output.WriteLineAsync(res);
            await console.Output.WriteLineAsync(response.StatusCode.ToString());
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync("nope");
                return;
            }

            await console.Output.WriteLineAsync("we gud");
        }

        public ApplyConfigCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}