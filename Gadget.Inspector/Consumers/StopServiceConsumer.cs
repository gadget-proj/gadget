using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.Contracts.Commands.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class StopServiceConsumer : IConsumer<IStopService>
    {
        private readonly ILogger<StopServiceConsumer> _logger;

        public StopServiceConsumer(ILogger<StopServiceConsumer> logger)
        {
            _logger = logger;
        }


        public Task Consume(ConsumeContext<IStopService> context)
        {
            _logger.LogInformation($"Trying to stop {context.Message.ServiceName}");
            var service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == context.Message.ServiceName);
            if (service == null)
            {
                throw new ApplicationException($"Service {context.Message.ServiceName} could not be found");
            }

            try
            {
                service.Refresh();
                service.Stop();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
            return Task.CompletedTask;
        }
    }
}