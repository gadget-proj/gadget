using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Gadget.Cli.Commands
{
    [Command("login")]
    public class LoginCommand : ICommand
    {
        [CommandParameter(0, Description = "username")]
        public string Username { get; set; }

        [CommandParameter(1, Description = "password")]
        public string Password { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await console.Output.WriteLineAsync($"{Username} {Password}");
        }
    }
}