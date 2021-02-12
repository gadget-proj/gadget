using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector.Consumers
{
    public class RestartServiceConsumer : IConsumer<IRestartService>
    {
        private readonly ILogger<RestartServiceConsumer> _logger;

        public RestartServiceConsumer(ILogger<RestartServiceConsumer> logger)
        {
            _logger = logger;
        }


        public Task Consume(ConsumeContext<IRestartService> context)
        {
            _logger.LogInformation($"Trying to start {context.Message.ServiceName}");
            var service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == context.Message.ServiceName);
            if (service == null)
            {
                throw new ApplicationException($"Service {context.Message.ServiceName} could not be found");
            }

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(500);

                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                else if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch
            {
                _logger.LogError($"Restart service error:{service.ServiceName}");
            }

            return Task.CompletedTask;
        }
    }
}