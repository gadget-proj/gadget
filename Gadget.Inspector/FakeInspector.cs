using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.Contracts.Events;
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
            var foo = new
            {
                Agent = Environment.MachineName,
                Services = Enumerable.Range(1, 5).Select(x => new
                    {Name = x.ToString(), Status = x % 2 == 0 ? "Running" : "Stopped"})
            };
            _logger.LogCritical(foo.Services.Count().ToString());
            await _publishEndpoint.Publish<IRegisterNewAgent>(foo, stoppingToken);
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