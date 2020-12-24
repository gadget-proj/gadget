using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class StartServiceConsumer : IConsumer<IStartService>
    {
        private readonly ILogger<StartServiceConsumer> _logger;

        public StartServiceConsumer(ILogger<StartServiceConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStartService> context)
        {
            _logger.LogInformation($"Trying to start {context.Message.ServiceName}");
            var service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == context.Message.ServiceName);
            if (service == null)
            {
                throw new ApplicationException($"Service {context.Message.ServiceName} could not be found");
            }

            service.Start();
        }
    }
}