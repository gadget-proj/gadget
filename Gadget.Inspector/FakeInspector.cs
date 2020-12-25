using System;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    public class FakeInspector : BackgroundService
    {
        private readonly ILogger<FakeInspector> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public FakeInspector(ILogger<FakeInspector> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Exec started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await _publishEndpoint.Publish<IServiceStatusChanged>(new
                {
                    Agent = Environment.MachineName,
                    Name = "serviceController.ServiceName",
                    Status = "current.ToString()"
                }, stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}