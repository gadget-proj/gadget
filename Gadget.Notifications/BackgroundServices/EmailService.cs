using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    public class EmailService : BackgroundService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly ChannelReader<EmailMessage> _channel;

        public EmailService(ILogger<EmailService> logger, Channel<EmailMessage> channel)
        {
            _logger = logger;
            _channel = channel.Reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.ReadAllAsync(stoppingToken))
            {
                _logger.LogCritical("Sending emails no cap");
            }
        }
    }
}