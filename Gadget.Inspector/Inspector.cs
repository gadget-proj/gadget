using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class Inspector : BackgroundService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<Inspector> _logger;
        public Inspector(IPublishEndpoint publishEndpoint, ILogger<Inspector> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("before");
            await _publishEndpoint.Publish<IRegisterNewAgent>(new
            {
                Agent = Environment.MachineName,
                // Services = ServiceController.GetServices().Select(s => new Service
                // {
                //     Name = s.ServiceName,
                //     Status = s.Status.ToString()
                // })
                Services = Enumerable.Empty<object>()
            }, stoppingToken);
            _logger.LogInformation("after");
        }
    }
}