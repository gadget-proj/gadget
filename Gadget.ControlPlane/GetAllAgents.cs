using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gadget.ControlPlane
{
    public class GetAllAgents : ICommand

    {
        private readonly HttpClient _httpClient;

        public GetAllAgents()
        {
            _httpClient = new HttpClient();
        }

        public async Task Execute()
        {
            var agents = await _httpClient.GetStringAsync("https://unfold.azurewebsites.net/agents");
            Console.WriteLine(agents);
        }
    }
}