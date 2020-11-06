using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class ServicesWatcher : BackgroundService
    {
        private readonly ILogger<ServicesWatcher> _logger;
        private readonly Channel<ServiceStatusChanged> _channel;

        public ServicesWatcher(ILogger<ServicesWatcher> logger, Channel<ServiceStatusChanged> channel)
        {
            _logger = logger;
            _channel = channel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _channel.Reader.WaitToReadAsync(stoppingToken);
                await foreach (var s in _channel.Reader.ReadAllAsync(stoppingToken))
                    _logger.LogInformation($"handing {s.Status} change for service {s.Name} on agent {s.AgentId}");
            }
        }
    }
}