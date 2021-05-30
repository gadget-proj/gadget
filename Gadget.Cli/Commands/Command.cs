using System.Net.Http;

namespace Gadget.Cli.Commands
{
    public class Command
    {
        protected readonly HttpClient HttpClient;

        public Command(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}