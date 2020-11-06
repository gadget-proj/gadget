using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Services
{
    public class ServicesWatcher : BackgroundService
    {
        private readonly Channel<string> _events;
        private readonly ILogger<ServicesWatcher> _logger;

        public ServicesWatcher(Channel<string> events, ILogger<ServicesWatcher> logger)
        {
            _events = events;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _events.Reader.WaitToReadAsync(stoppingToken);
                await foreach (var s in _events.Reader.ReadAllAsync(stoppingToken))
                    _logger.LogInformation($"handing {s}");
            }
        }
    }
}