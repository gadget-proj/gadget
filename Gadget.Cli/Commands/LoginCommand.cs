using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    public record LoginRequest(string Username, string Password);

    [Command("account login", Description = "lets you login to your cluster")]
    public class LoginCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "username")]
        public string Username { get; set; }

        [CommandParameter(1, Description = "password")]
        public string Password { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var request = new LoginRequest(Username, Password);
            var response = await HttpClient.PostAsJsonAsync("http://localhost:5002/auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync(response.ReasonPhrase);
                return;
            }

            var token = await response.Content.ReadAsStringAsync();
            Environment.SetEnvironmentVariable("GADGET_TOKEN", token);
            await console.Output.WriteLineAsync("Logged in");
        }

        public LoginCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}