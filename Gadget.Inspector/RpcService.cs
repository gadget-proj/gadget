using System.Threading;
using System.Threading.Tasks;
using GadgetRPC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class RpcService : BackgroundService
    {
        private readonly GadgetRPC.Gadget.GadgetClient _client;
        private readonly ILogger<RpcService> _logger;

        public RpcService(GadgetRPC.Gadget.GadgetClient client, ILogger<RpcService> logger)
        {
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var response = await _client.TestAsync(new TestRequest(), cancellationToken: stoppingToken);
            _logger.LogInformation(response.Message);
        }
    }
}