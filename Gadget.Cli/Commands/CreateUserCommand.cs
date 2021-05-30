using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    public record CreateUserRequest(string Username, string Password);

    [Command("account create",
        Description = "lets you create an account that will later be used to administer your cluster")]
    public class CreateUserCommand : Command, ICommand
    {
        [CommandParameter(0, Description = "username")]
        public string Username { get; set; }

        [CommandParameter(1, Description = "password")]
        public string Password { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var request = new CreateUserRequest(Username, Password);
            var response = await HttpClient.PostAsJsonAsync("http://localhost:5002/auth/new", request);
            if (!response.IsSuccessStatusCode)
            {
                await console.Output.WriteLineAsync("Could not create account");
                return;
            }

            await console.Output.WriteLineAsync("Account created, you can now login");
        }

        public CreateUserCommand(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}