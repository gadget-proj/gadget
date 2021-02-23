using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Gadget.Notifications.Domain.ValueObjects;
using Gadget.Notifications.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.BackgroundServices
{
    public class EmailService : BackgroundService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailService _emailService;
        private readonly ChannelReader<EmailMessage> _channel;

        public EmailService(
            ILogger<EmailService> logger, 
            Channel<EmailMessage> channel,
            IEmailService emailService)
        {
            _logger = logger;
            _channel = channel.Reader;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _channel.ReadAllAsync(stoppingToken))
            {
                await _emailService.SendEmailMessage(message, stoppingToken);
            }
        }
    }
}